// Copyright (c) Rodel. All rights reserved.

using System.Reflection;
using RodelAgent.Interfaces;
using RodelAgent.Models.Abstractions;
using RodelAgent.Models.Constants;
using RodelChat.Models.Client;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 聊天服务选项面板.
/// </summary>
public sealed partial class ChatProviderOptionPanel : ChatProviderOptionPanelBase
{
    /// <summary>
    /// <see cref="IsMaxRoundEnabled"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsMaxRoundEnabledProperty =
        DependencyProperty.Register(nameof(IsMaxRoundEnabled), typeof(bool), typeof(ChatProviderOptionPanel), new PropertyMetadata(true));

    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatProviderOptionPanel"/> class.
    /// </summary>
    public ChatProviderOptionPanel() => InitializeComponent();

    /// <summary>
    /// 属性改变事件.
    /// </summary>
    public event EventHandler PropertyChanged;

    /// <summary>
    /// 获取或设置是否启用最大轮数.
    /// </summary>
    public bool IsMaxRoundEnabled
    {
        get => (bool)GetValue(IsMaxRoundEnabledProperty);
        set => SetValue(IsMaxRoundEnabledProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        _isInitialized = false;
        if (IsLoaded)
        {
            Initialize();
        }
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => Initialize();

    private void Initialize()
    {
        if (ViewModel?.Parameters == null)
        {
            return;
        }

        StreamOutputSwitch.IsOn = ViewModel.UseStreamOutput;
        MaxTurnSlider.Value = ViewModel.MaxRounds;
        CustomPanel.Children.Clear();
        var properties = ViewModel.Parameters.GetType().GetProperties();
        for (var i = 0; i < properties.Length; i++)
        {
            var property = properties[i];
            if (Attribute.GetCustomAttribute(property, typeof(BaseFieldAttribute)) is BaseFieldAttribute attribute)
            {
                var ft = attribute.FieldType;
                var ele = ft switch
                {
                    ParameterFieldType.Boolean => CreateBooleanElement(attribute, property),
                    ParameterFieldType.RangeFloat => CreateRangeFloatElement(attribute, property),
                    ParameterFieldType.RangeInt => CreateRangeIntElement(attribute, property),
                    ParameterFieldType.RangeLong => CreateRangeLongElement(attribute, property),
                    ParameterFieldType.Text => CreateTextElement(attribute, property),
                    ParameterFieldType.Selection => CreateSelectionElement(attribute, property),
                    _ => null,
                };

                if (ele != null)
                {
                    if (i == properties.Length - 2)
                    {
                        if (ele is Grid grid)
                        {
                            grid.BorderThickness = new Thickness(0);
                        }
                        else if (ele is StackPanel panel)
                        {
                            panel.BorderThickness = new Thickness(0);
                        }
                    }

                    CustomPanel.Children.Add(ele);
                }
            }
        }

        _isInitialized = true;
    }

    private FrameworkElement CreateBooleanElement(BaseFieldAttribute attr, PropertyInfo property)
    {
        var boolAttr = attr as BooleanFieldAttribute;
        var value = property.GetValue(ViewModel.Parameters) as bool?;
        var toggleSwitch = new ToggleSwitch
        {
            IsOn = value ?? false,
            MinWidth = 0,
        };

        toggleSwitch.Toggled += (s, e) =>
        {
            property.SetValue(ViewModel.Parameters, toggleSwitch.IsOn);
            ViewModel.Parameters.ToDictionary();
            PropertyChanged?.Invoke(this, EventArgs.Empty);
        };

        var item = ItemTemplate.LoadContent() as Grid;
        item.Padding = new Thickness(0, 2, 0, 2);
        SetPropertyName(item.Children.First() as TextBlock, property.Name);
        (item.Children.Last() as ContentPresenter).Content = toggleSwitch;
        return item;
    }

    private FrameworkElement CreateRangeFloatElement(BaseFieldAttribute attr, PropertyInfo property)
    {
        var rangeAttr = attr as RangeFloatFieldAttribute;
        var value = property.GetValue(ViewModel.Parameters) as double?;
        var min = rangeAttr.Minimum;
        var max = rangeAttr.Maximum;

        object innerElement = null;
        if (max - min > 10)
        {
            // Use numberbox
            var numberBox = new NumberBox
            {
                Minimum = min,
                Maximum = max,
                Value = value ?? min,
                SmallChange = 0.1,
                LargeChange = 1,
                MinWidth = 120,
                SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact,
            };

            numberBox.ValueChanged += (s, e) =>
            {
                property.SetValue(ViewModel.Parameters, numberBox.Value);
                ViewModel.Parameters.ToDictionary();
                PropertyChanged?.Invoke(this, EventArgs.Empty);
            };

            innerElement = numberBox;
        }
        else
        {
            var slider = new Slider
            {
                Minimum = rangeAttr.Minimum,
                Maximum = rangeAttr.Maximum,
                Value = value ?? rangeAttr.Minimum,
                StepFrequency = 0.1,
                Width = 120,
            };

            slider.ValueChanged += (s, e) =>
            {
                property.SetValue(ViewModel.Parameters, slider.Value);
                ViewModel.Parameters.ToDictionary();
                PropertyChanged?.Invoke(this, EventArgs.Empty);
            };

            innerElement = slider;
        }

        var item = ItemTemplate.LoadContent() as Grid;
        SetPropertyName(item.Children.First() as TextBlock, property.Name);
        (item.Children.Last() as ContentPresenter).Content = innerElement;
        return item;
    }

    private FrameworkElement CreateRangeIntElement(BaseFieldAttribute attr, PropertyInfo property)
    {
        var rangeAttr = attr as RangeIntFieldAttribute;
        var value = property.GetValue(ViewModel.Parameters) as int?;
        var min = rangeAttr.Minimum;
        var max = rangeAttr.Maximum;
        object innerElement = null;

        if (max - min > 10)
        {
            // Use numberbox
            var numberBox = new NumberBox
            {
                Minimum = min,
                Maximum = max,
                Value = value ?? min,
                SmallChange = 1,
                LargeChange = 10,
                MinWidth = 120,
                SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact,
            };

            numberBox.ValueChanged += (s, e) =>
            {
                property.SetValue(ViewModel.Parameters, Convert.ToInt32(numberBox.Value));
                ViewModel.Parameters.ToDictionary();
                PropertyChanged?.Invoke(this, EventArgs.Empty);
            };

            innerElement = numberBox;
        }
        else
        {
            var slider = new Slider
            {
                Minimum = rangeAttr.Minimum,
                Maximum = rangeAttr.Maximum,
                Value = value ?? rangeAttr.Minimum,
                StepFrequency = 1,
                Width = 120,
            };

            slider.ValueChanged += (s, e) =>
            {
                property.SetValue(ViewModel.Parameters, (int)slider.Value);
                ViewModel.Parameters.ToDictionary();
                PropertyChanged?.Invoke(this, EventArgs.Empty);
            };

            innerElement = slider;
        }

        var item = ItemTemplate.LoadContent() as Grid;
        SetPropertyName(item.Children.First() as TextBlock, property.Name);
        (item.Children.Last() as ContentPresenter).Content = innerElement;
        return item;
    }

    private FrameworkElement CreateRangeLongElement(BaseFieldAttribute attr, PropertyInfo property)
    {
        var rangeAttr = attr as RangeLongFieldAttribute;
        var value = property.GetValue(ViewModel.Parameters) as long?;
        var min = rangeAttr.Minimum;
        var max = rangeAttr.Maximum;
        var numberBox = new NumberBox
        {
            Minimum = min,
            Maximum = max,
            Value = value ?? min,
            SmallChange = 1,
            LargeChange = 10,
            MinWidth = 120,
            SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact,
        };

        numberBox.ValueChanged += (s, e) =>
        {
            property.SetValue(ViewModel.Parameters, Convert.ToInt64(numberBox.Value));
            ViewModel.Parameters.ToDictionary();
            PropertyChanged?.Invoke(this, EventArgs.Empty);
        };

        var item = ItemTemplate.LoadContent() as Grid;
        SetPropertyName(item.Children.First() as TextBlock, property.Name);
        (item.Children.Last() as ContentPresenter).Content = numberBox;
        return item;
    }

    private FrameworkElement CreateSelectionElement(BaseFieldAttribute attr, PropertyInfo property)
    {
        var selectionAttr = attr as SelectionFieldAttribute;
        var value = property.GetValue(ViewModel.Parameters) as string;
        var comboBox = new ComboBox
        {
            ItemsSource = selectionAttr.Options,
            SelectedItem = value ?? selectionAttr.Options.FirstOrDefault(),
            MinWidth = 120,
        };

        comboBox.SelectionChanged += (s, e) =>
        {
            property.SetValue(ViewModel.Parameters, comboBox.SelectedItem);
            ViewModel.Parameters.ToDictionary();
            PropertyChanged?.Invoke(this, EventArgs.Empty);
        };

        var item = ItemTemplate.LoadContent() as Grid;
        SetPropertyName(item.Children.First() as TextBlock, property.Name);
        (item.Children.Last() as ContentPresenter).Content = comboBox;
        return item;
    }

    private FrameworkElement CreateTextElement(BaseFieldAttribute attr, PropertyInfo property)
    {
        var value = property.GetValue(ViewModel.Parameters) as string;

        var panel = TextTemplate.LoadContent() as StackPanel;
        var box = panel.Children.OfType<TextBox>().First();
        box.Text = value;
        box.TextChanged += (s, e) =>
        {
            property.SetValue(ViewModel.Parameters, box.Text);
            ViewModel.Parameters.ToDictionary();
            PropertyChanged?.Invoke(this, EventArgs.Empty);
        };

        SetPropertyName(panel.Children.First() as TextBlock, property.Name);
        return panel;
    }

    private void OnStreamOutputChanged(object sender, RoutedEventArgs e)
    {
        if (!_isInitialized)
        {
            return;
        }

        ViewModel.UseStreamOutput = StreamOutputSwitch.IsOn;
        PropertyChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SetPropertyName(TextBlock textBlock, string name)
    {
        var text = this.Get<IStringResourceToolkit>().GetString(name);
        if (string.IsNullOrEmpty(text))
        {
            text = name;
        }

        var tip = this.Get<IStringResourceToolkit>().GetString($"{name}Description");
        if (!string.IsNullOrEmpty(tip))
        {
            ToolTipService.SetToolTip(textBlock, tip);
        }

        textBlock.Text = text;
    }

    private void OnChatTruenChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (!_isInitialized)
        {
            return;
        }

        ViewModel.MaxRounds = Convert.ToInt32(MaxTurnSlider.Value);
        PropertyChanged?.Invoke(this, EventArgs.Empty);
    }
}

/// <summary>
/// 聊天服务选项面板基类.
/// </summary>
public abstract class ChatProviderOptionPanelBase : LayoutUserControlBase<ChatSessionPreset>
{
}
