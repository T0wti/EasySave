using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using EasySave.Application.DTOs;

namespace EasySave.GUI.Views;

public partial class BackupJobsList : UserControl
{
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

    public IEnumerable<BackupJobDTO> Jobs
    {
        get => GetValue(JobsProperty);
        set
        {
            Console.WriteLine($"[BackupJobsList] Jobs setter called with {value?.Count() ?? 0} items");
            if (value != null)
            {
                foreach (var job in value)
                {
                    Console.WriteLine($"  - {job.Name}");
                }
            }
            SetValue(JobsProperty, value);
        }
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
        Console.WriteLine("[BackupJobsList] Constructor START");
        InitializeComponent();
        Console.WriteLine("[BackupJobsList] Constructor END");
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        Console.WriteLine($"[BackupJobsList] OnAttachedToVisualTree - Jobs count: {Jobs?.Count() ?? 0}");
        
        // Vérifier que la ListBox voit bien les données
        var listBox = this.FindControl<ListBox>("JobsListBox");
        if (listBox != null)
        {
            Console.WriteLine($"[BackupJobsList] ListBox.ItemsSource count: {(listBox.ItemsSource as IEnumerable<BackupJobDTO>)?.Count() ?? 0}");
        }
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        
        if (change.Property == JobsProperty)
        {
            var jobs = change.NewValue as IEnumerable<BackupJobDTO>;
            var count = jobs?.Count() ?? 0;
            Console.WriteLine($"[BackupJobsList] OnPropertyChanged - JobsProperty changed: {count} items");
        }
    }
}