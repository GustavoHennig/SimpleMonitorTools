using ReactiveUI;
using SimpleMonitorTools.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace SimpleMonitorTools
{
    public class PostLaunchStepEditorViewModel : ReactiveObject
    {
        private ObservableCollection<PostLaunchStep> _postLaunchSteps;
        public ObservableCollection<PostLaunchStep> PostLaunchSteps
        {
            get => _postLaunchSteps;
            set => this.RaiseAndSetIfChanged(ref _postLaunchSteps, value);
        }

        private PostLaunchStep _selectedStep;
        public PostLaunchStep SelectedStep
        {
            get => _selectedStep;
            set => this.RaiseAndSetIfChanged(ref _selectedStep, value);
        }

        public List<PostLaunchStepType> StepTypes { get; } = Enum.GetValues(typeof(PostLaunchStepType)).Cast<PostLaunchStepType>().ToList();

        // New step fields
        private PostLaunchStepType _newStepType;
        public PostLaunchStepType NewStepType
        {
            get => _newStepType;
            set => this.RaiseAndSetIfChanged(ref _newStepType, value);
        }

        private string _newWindowTitle;
        public string NewWindowTitle
        {
            get => _newWindowTitle;
            set => this.RaiseAndSetIfChanged(ref _newWindowTitle, value);
        }

        private string _newControlType;
        public string NewControlType
        {
            get => _newControlType;
            set => this.RaiseAndSetIfChanged(ref _newControlType, value);
        }

        private string _newName;
        public string NewName
        {
            get => _newName;
            set => this.RaiseAndSetIfChanged(ref _newName, value);
        }

        private string _newNameMatchMode;
        public string NewNameMatchMode
        {
            get => _newNameMatchMode;
            set => this.RaiseAndSetIfChanged(ref _newNameMatchMode, value);
        }

        private int _newDurationMs;
        public int NewDurationMs
        {
            get => _newDurationMs;
            set => this.RaiseAndSetIfChanged(ref _newDurationMs, value);
        }

        public ReactiveCommand<Unit, Unit> AddStepCommand { get; }
        public ReactiveCommand<PostLaunchStep, Unit> RemoveStepCommand { get; }

        public PostLaunchStepEditorViewModel(IEnumerable<PostLaunchStep> steps = null)
        {
            PostLaunchSteps = steps != null
                ? new ObservableCollection<PostLaunchStep>(steps)
                : new ObservableCollection<PostLaunchStep>();

            AddStepCommand = ReactiveCommand.Create(AddStep);
            RemoveStepCommand = ReactiveCommand.Create<PostLaunchStep>(RemoveStep);

            NewStepType = StepTypes.FirstOrDefault();
        }

        private void AddStep()
        {
            var step = new PostLaunchStep
            {
                StepType = NewStepType,
                WindowTitle = NewWindowTitle,
                ControlType = NewControlType,
                Name = NewName,
                NameMatchMode = NewNameMatchMode,
                DurationMs = NewDurationMs
            };

            PostLaunchSteps.Add(step);

            // Reset fields
            NewWindowTitle = string.Empty;
            NewControlType = string.Empty;
            NewName = string.Empty;
            NewNameMatchMode = string.Empty;
            NewDurationMs = 0;
        }

        private void RemoveStep(PostLaunchStep step)
        {
            if (step != null)
                PostLaunchSteps.Remove(step);
        }
    }
}
