namespace momentum_app.Pages;

public partial class TabNav : TabbedPage
{
	public TabNav()
	{
		InitializeComponent();

		var paginaPomodoro = new HomePage()
		{
			Title = "TIMER",
			IconImageSource = "timericon.svg"
		};

        var paginaTarefas = new TaskPage()
        {
            Title = "TASK",
            IconImageSource = "taskicon.svg"
        };

        var paginaDados = new DataPage()
        {
            Title = "DATA",
            IconImageSource = "dataicon.svg"
        };

        var paginaConfig = new ConfigPage()
        {
            Title = "CONFIG",
            IconImageSource = "configicon.svg"
        };

        this.Children.Add(paginaPomodoro);
        this.Children.Add(paginaTarefas);
        this.Children.Add(paginaDados);
        this.Children.Add(paginaConfig);

    }
}