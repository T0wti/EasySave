using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using EasySave.Application.DTOs;
using EasySave.Application.Resources;

namespace EasySave.GUI.Views;

public partial class BackupJobsList : UserControl
{
    public static readonly StyledProperty<ITextProvider> TextsProperty =
        AvaloniaProperty.Register<BackupJobsList, ITextProvider>(nameof(Texts));

    public static readonly StyledProperty<IEnumerable<BackupJobDTO>> JobsProperty =
        AvaloniaProperty.Register<BackupJobsList, IEnumerable<BackupJobDTO>>(
            nameof(Jobs));
    
    public static readonly StyledProperty<BackupJobDTO> SelectedJobProperty =
        AvaloniaProperty.Register<BackupJobsList, BackupJobDTO>(
            nameof(SelectedJob),
            defaultBindingMode: BindingMode.TwoWay);
    
    public static readonly StyledProperty<ICommand> JobSelectedCommandProperty =
        AvaloniaProperty.Register<BackupJobsList, ICommand>(
            nameof(JobSelectedCommand));

    public ITextProvider Texts
    {
        get => GetValue(TextsProperty);
        set => SetValue(TextsProperty, value);
    }
    public IEnumerable<BackupJobDTO> Jobs
    {
        get => GetValue(JobsProperty);
        set => SetValue(JobsProperty, value);
    }

    public BackupJobDTO SelectedJob
    {
        get => GetValue(SelectedJobProperty);
        set => SetValue(SelectedJobProperty, value);
    }

    public ICommand JobSelectedCommand
    {
        get => GetValue(JobSelectedCommandProperty);
        set => SetValue(JobSelectedCommandProperty, value);
    }

    public BackupJobsList()
    {
        InitializeComponent();
    }
}