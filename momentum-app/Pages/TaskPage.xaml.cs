using System.Linq;

namespace momentum_app.Pages
{
    public partial class TaskPage : ContentPage
    {
        public TaskPage()
        {
            InitializeComponent();
        }

        private async void btAddTask_Clicked(object sender, EventArgs e)
        {
            string taskName = await DisplayPromptAsync("New task", "What's your new task?", "Add", "Cancel");

            if (!string.IsNullOrWhiteSpace(taskName))
            {
                string weight = await DisplayActionSheet("Set Weight", "Cancel", null, "High", "Medium", "Low");

                if (weight == "Cancel" || string.IsNullOrEmpty(weight))
                {
                    return;
                }

                var taskView = CreateTaskView(taskName, weight);
                InsertTaskViewSorted(taskView);
                UpdateTaskStatus();
            }
        }
        private HorizontalStackLayout CreateTaskView(string taskName, string weight)
        {
            var checkBox = new CheckBox
            {
                Color = Color.FromArgb("#6d4b96"),
                VerticalOptions = LayoutOptions.Center
            };

            var taskLabel = new Label
            {
                Text = taskName,
                TextColor = Colors.White,
                FontFamily = "Montserrat",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnEditTaskClicked;
            taskLabel.GestureRecognizers.Add(tapGesture);

            var weightLabel = new Label
            {
                Text = weight,
                TextColor = GetColorForWeight(weight),
                FontFamily = "MontserratBold",
                FontSize = 10,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(5, 0)
            };

            var deleteButton = new Button
            {
                Text = "X",
                TextColor = Color.FromArgb("#fff"),
                FontFamily = "MontserratBold",
                FontSize = 12,
                BackgroundColor = Color.FromArgb("#ff6b6b"),
                BorderColor = Color.FromArgb("#ff6b6b"),
                BorderWidth = 1,
                CornerRadius = 30,
                HeightRequest = 10
            };

            var taskView = new HorizontalStackLayout
            {
                Spacing = 10,
                Padding = new Thickness(20, 10),
                StyleId = "taskItem"
            };

            taskView.Children.Add(checkBox);
            taskView.Children.Add(taskLabel);
            taskView.Children.Add(weightLabel);
            taskView.Children.Add(deleteButton);

            checkBox.CheckedChanged += OnTaskCheckedChanged;
            deleteButton.Clicked += OnDeleteTaskClicked;

            return taskView;
        }

        private void InsertTaskViewSorted(HorizontalStackLayout taskView)
        {
            var mainLayout = (VerticalStackLayout)this.Content;
            int newTaskWeightValue = GetWeightValue(GetWeightFromTaskView(taskView));

            int buttonIndex = mainLayout.Children.IndexOf(btAddTask);
            if (buttonIndex == -1)
            {
                buttonIndex = mainLayout.Children.Count;
            }

            int insertAt = buttonIndex;

            for (int i = 0; i < buttonIndex; i++)
            {
                var child = mainLayout.Children[i];
                if (child is HorizontalStackLayout existingTaskView && existingTaskView.StyleId == "taskItem")
                {
                    int existingTaskWeightValue = GetWeightValue(GetWeightFromTaskView(existingTaskView));

                    if (newTaskWeightValue > existingTaskWeightValue)
                    {
                        insertAt = i;
                        break;
                    }
                }
            }

            mainLayout.Children.Insert(insertAt, taskView);
        }

        private int GetWeightValue(string weight)
        {
            switch (weight)
            {
                case "High": return 3;
                case "Medium": return 2;
                case "Low": return 1;
                default: return 0;
            }
        }

        private string GetWeightFromTaskView(HorizontalStackLayout taskView)
        {
            if (taskView.Children[2] is Label weightLabel)
            {
                return weightLabel.Text;
            }
            return null;
        }


        private void OnTaskCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var taskView = (HorizontalStackLayout)checkBox.Parent;

            if (taskView.Children[1] is Label taskLabel && taskView.Children[2] is Label weightLabel)
            {
                if (e.Value)
                {
                    taskLabel.TextDecorations = TextDecorations.Strikethrough;
                    taskLabel.TextColor = Colors.Gray;
                    weightLabel.TextColor = Colors.Gray;

                    if (checkBox.AutomationId != "counted")
                    {
                        var totalTasks = Preferences.Get("TotalTasksCompleted", 0);
                        Preferences.Set("TotalTasksCompleted", totalTasks + 1);
                        checkBox.AutomationId = "counted";
                    }
                }
                else
                {
                    taskLabel.TextDecorations = TextDecorations.None;
                    taskLabel.TextColor = Colors.White;
                    weightLabel.TextColor = GetColorForWeight(weightLabel.Text);
                }
            }
        }

        private async void OnEditTaskClicked(object sender, EventArgs e)
        {
            var taskLabel = (Label)sender;
            var taskView = (HorizontalStackLayout)taskLabel.Parent;
            var mainLayout = (VerticalStackLayout)taskView.Parent;

            if (!(taskView.Children[0] is CheckBox checkBox) ||
                !(taskView.Children[2] is Label weightLabel))
            {
                return;
            }

            string currentName = taskLabel.Text;
            bool wasChecked = checkBox.IsChecked;

            string newName = await DisplayPromptAsync("Edit", "Edit the task name: ", "Save", "Cancel", initialValue: currentName);

            if (string.IsNullOrWhiteSpace(newName))
            {
                return;
            }

            string newWeight = await DisplayActionSheet("Edit Weight", "Cancel", null, "High", "Medium", "Low");

            if (newWeight == "Cancel" || string.IsNullOrEmpty(newWeight))
            {
                return;
            }

            mainLayout.Children.Remove(taskView);

            var newTaskView = CreateTaskView(newName, newWeight);
            InsertTaskViewSorted(newTaskView);

            if (newTaskView.Children[0] is CheckBox newCheckBox)
            {
                if (wasChecked)
                {
                    newCheckBox.AutomationId = "counted";
                }

                newCheckBox.IsChecked = wasChecked;
            }
        }
        private void OnDeleteTaskClicked(object sender, EventArgs e)
        {
            var deleteButton = (Button)sender;
            var taskView = (HorizontalStackLayout)deleteButton.Parent;
            var mainLayout = (VerticalStackLayout)taskView.Parent;

            mainLayout.Children.Remove(taskView);

            UpdateTaskStatus();
        }

        private void UpdateTaskStatus()
        {
            var mainLayout = (VerticalStackLayout)this.Content;

            int taskCount = mainLayout.Children
                .OfType<View>()
                .Count(c => c.StyleId == "taskItem");

            if (this.FindByName("lbStatusTask") is Label statusLabel)
            {
                statusLabel.IsVisible = (taskCount == 0);
            }
        }

        private Color GetColorForWeight(string weight)
        {
            switch (weight)
            {
                case "High":
                    return Color.FromArgb("#ff6b6b");
                case "Medium":
                    return Color.FromArgb("#ffc107");
                case "Low":
                    return Color.FromArgb("#28a745");
                default:
                    return Colors.Gray;
            }
        }
    }
}