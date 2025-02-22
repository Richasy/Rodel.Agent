﻿// Copyright (c) Richasy. All rights reserved.

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace RodelAgent.UI.Extensions;

/// <summary>
/// AudioFileReader simplifies opening an audio file in NAudio
/// Simply pass in the filename, and it will attempt to open the
/// file and set up a conversion path that turns into PCM IEEE float.
/// ACM codecs will be used for conversion.
/// It provides a volume property and implements both WaveStream and
/// ISampleProvider, making it possibly the only stage in your audio
/// pipeline necessary for simple playback scenarios
/// </summary>
public partial class AudioFileReader : WaveStream, ISampleProvider
{
    private WaveStream? readerStream; // the waveStream which we will use for all positioning
    private readonly SampleChannel sampleChannel; // sample provider that gives us most stuff we need
    private readonly int destBytesPerSample;
    private readonly int sourceBytesPerSample;
    private readonly long length;
    private readonly Lock lockObject;

    /// <summary>
    /// Initializes a new instance of AudioFileReader
    /// </summary>
    /// <param name="fileName">The file to open</param>
    public AudioFileReader(string fileName)
    {
        lockObject = new Lock();
        FileName = fileName;
        CreateReaderStream(fileName);
        sourceBytesPerSample = readerStream!.WaveFormat.BitsPerSample / 8 * readerStream.WaveFormat.Channels;
        sampleChannel = new SampleChannel(readerStream, false);
        destBytesPerSample = 4 * sampleChannel.WaveFormat.Channels;
        length = SourceToDest(readerStream.Length);
    }

    /// <summary>
    /// Creates the reader stream, supporting all filetypes in the core NAudio library,
    /// and ensuring we are in PCM format
    /// </summary>
    /// <param name="fileName">File Name</param>
    private void CreateReaderStream(string fileName)
    {
        if (fileName.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
        {
            readerStream = new WaveFileReader(fileName);
        }
        else if(fileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
        {
            readerStream = new Mp3FileReaderBase(File.OpenRead(fileName), CreateAcmFrameDecompressor);
        }

        static IMp3FrameDecompressor CreateAcmFrameDecompressor(WaveFormat mp3Format)
        {
            // new DmoMp3FrameDecompressor(this.Mp3WaveFormat); 
            return new AcmMp3FrameDecompressor(mp3Format);
        }
    }
    /// <summary>
    /// File Name
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// WaveFormat of this stream
    /// </summary>
    public override WaveFormat WaveFormat => sampleChannel.WaveFormat;

    /// <summary>
    /// Length of this stream (in bytes)
    /// </summary>
    public override long Length => length;

    /// <summary>
    /// Position of this stream (in bytes)
    /// </summary>
    public override long Position
    {
        get => SourceToDest(readerStream?.Position ?? 0);
        set { lock (lockObject) { readerStream!.Position = DestToSource(value); } }
    }

    /// <summary>
    /// Reads from this wave stream
    /// </summary>
    /// <param name="buffer">Audio buffer</param>
    /// <param name="offset">Offset into buffer</param>
    /// <param name="count">Number of bytes required</param>
    /// <returns>Number of bytes read</returns>
    public override int Read(byte[] buffer, int offset, int count)
    {
        var waveBuffer = new WaveBuffer(buffer);
        var samplesRequired = count / 4;
        var samplesRead = Read(waveBuffer.FloatBuffer, offset / 4, samplesRequired);
        return samplesRead * 4;
    }

    /// <summary>
    /// Reads audio from this sample provider
    /// </summary>
    /// <param name="buffer">Sample buffer</param>
    /// <param name="offset">Offset into sample buffer</param>
    /// <param name="count">Number of samples required</param>
    /// <returns>Number of samples read</returns>
    public int Read(float[] buffer, int offset, int count)
    {
        lock (lockObject)
        {
            return sampleChannel.Read(buffer, offset, count);
        }
    }

    /// <summary>
    /// Gets or Sets the Volume of this AudioFileReader. 1.0f is full volume
    /// </summary>
    public float Volume
    {
        get => sampleChannel.Volume; set => sampleChannel.Volume = value;
    }

    /// <summary>
    /// Helper to convert source to dest bytes
    /// </summary>
    private long SourceToDest(long sourceBytes)
        => destBytesPerSample * (sourceBytes / sourceBytesPerSample);

    /// <summary>
    /// Helper to convert dest to source bytes
    /// </summary>
    private long DestToSource(long destBytes)
        => sourceBytesPerSample * (destBytes / destBytesPerSample);

    /// <summary>
    /// Disposes this AudioFileReader
    /// </summary>
    /// <param name="disposing">True if called from Dispose</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (readerStream != null)
            {
                readerStream.Dispose();
                readerStream = null;
            }
        }

        base.Dispose(disposing);
    }
}
