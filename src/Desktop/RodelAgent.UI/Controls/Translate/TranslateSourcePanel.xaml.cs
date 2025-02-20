// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Input;
using System.ComponentModel;
using Windows.System;
using Windows.UI.Core;

namespace RodelAgent.UI.Controls.Translate;

/// <summary>
/// 翻译源面板.
/// </summary>
public sealed partial class TranslateSourcePanel : TranslatePageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateSourcePanel"/> class.
    /// </summary>
    public TranslateSourcePanel() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        CheckTextLength();
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.IsExceedLimit))
        {
            CheckTextLength();
        }
    }

    private void CheckTextLength()
    {
        var stateName = ViewModel.IsExceedLimit ? nameof(ExceedState) : nameof(NormalState);
        VisualStateManager.GoToState(this, stateName, false);
    }

    private void OnInputBoxPreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter)
        {
            var shiftState = InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift);
            var isShiftDown = shiftState == CoreVirtualKeyStates.Down || shiftState == (CoreVirtualKeyStates.Down | CoreVirtualKeyStates.Locked);

            if (!isShiftDown)
            {
                e.Handled = true;
                ViewModel.StartTranslateCommand.Execute(default);
            }
        }
    }

    private void OnSourceLanguageChanged(object sender, SelectionChangedEventArgs e)
    {
        var language = (sender as ComboBox)?.SelectedItem as ViewModels.Items.TranslateLanguageItemViewModel;
        if (language == null || ViewModel.SelectedSourceLanguage == language)
        {
            return;
        }

        ViewModel.SelectedSourceLanguage = language;
    }
}
