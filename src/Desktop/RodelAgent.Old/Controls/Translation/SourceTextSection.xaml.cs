// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Input;
using RodelAgent.UI.ViewModels.Components;
using RodelAgent.UI.ViewModels.Items;
using System.ComponentModel;
using Windows.System;
using Windows.UI.Core;

namespace RodelAgent.UI.Controls.Translation;

/// <summary>
/// 源文本区域.
/// </summary>
public sealed partial class SourceTextSection : TranslateSessionControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SourceTextSection"/> class.
    /// </summary>
    public SourceTextSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(TranslateSessionViewModel? oldValue, TranslateSessionViewModel? newValue)
    {
        if (oldValue is TranslateSessionViewModel oldVM)
        {
            oldVM.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (newValue is TranslateSessionViewModel newVM)
        {
            newVM.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded() => CheckTextLength();

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel != null)
        {
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.IsExceedMaxTextLength))
        {
            CheckTextLength();
        }
    }

    private void OnInputBoxPreviewKeyDownAsync(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter)
        {
            var shiftState = InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift);
            var isShiftDown = shiftState == CoreVirtualKeyStates.Down || shiftState == (CoreVirtualKeyStates.Down | CoreVirtualKeyStates.Locked);

            if (!isShiftDown)
            {
                e.Handled = true;
                ViewModel.TranslateCommand.Execute(default);
            }
        }
    }

    private void CheckTextLength()
    {
        var stateName = ViewModel.IsExceedMaxTextLength ? nameof(ExceedState) : nameof(NormalState);
        VisualStateManager.GoToState(this, stateName, false);
    }

    private void OnSourceLanguageChanged(object sender, SelectionChangedEventArgs e)
    {
        var language = (sender as ComboBox)?.SelectedItem as TranslateLanguageItemViewModel;
        if (language == null || ViewModel.SourceLanguage == language)
        {
            return;
        }

        ViewModel.SourceLanguage = language;
    }
}
