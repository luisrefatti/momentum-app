namespace momentum_app.Pages;

public partial class TabNav : TabbedPage
{
	public TabNav()
	{
		InitializeComponent();

		var paginaPomodoro = new HomePage()
		{
			Title = "Timer",
			IconImageSource = ""
		};

        var paginaTarefas = new TaskPage()
        {
            Title = "Task",
            IconImageSource = ""
        };

        var paginaDados = new DataPage()
        {
            Title = "Data",
            IconImageSource = ""
        };

        var paginaConfig = new ConfigPage()
        {
            Title = "Config",
            IconImageSource = ""
        };

        this.Children.Add(paginaPomodoro);
        this.Children.Add(paginaTarefas);
        this.Children.Add(paginaDados);
        this.Children.Add(paginaConfig);

    }
}