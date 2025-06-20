using Android.Text.Method;

namespace Icculus.PhysFS.NET;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_main);

        TextView? commandLine = FindViewById<TextView>(Resource.Id.command_line);
        TextView? outputField = FindViewById<TextView>(Resource.Id.output_field);
        Button? runCommand = FindViewById<Button>(Resource.Id.run_command);
        Button? clearLog = FindViewById<Button>(Resource.Id.clear_log);

        if (commandLine is null || outputField is null) return;
        if (runCommand is null || clearLog is null) return;

        outputField.MovementMethod = new ScrollingMovementMethod();
        outputField.Text = PhysFsTest.OutputArchivers();

        runCommand.Click += (obj, args) =>
        {
            string? line = commandLine.Text;
            if (string.IsNullOrEmpty(line)) FinishAffinity();

            string? commandResult = PhysFsTest.ProcessCommand(line!);
            if (commandResult == null) FinishAffinity();

            outputField.Text += commandResult + "\n\n";
        };

        clearLog.Click += (obj, args) =>
        {
            commandLine.Text = string.Empty;
            outputField.Text = string.Empty;
        };
    }
}
