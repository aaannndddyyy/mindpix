using System;
using System.Threading;
using Gtk;
using cognitivedynamics;

public partial class MainWindow: Gtk.Window
{	
	string filename = "/usr/bin/mindpixels.txt";
	dynamicstestyesno test;
	bool test_running;
	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();

		lblProposition.Text = "Press the start button below to begin the test.  Then click to answer each proposition.";
		test = new dynamicstestyesno(filename,400,300);
		//test.offset_x = imgTest.GdkWindow.Screen.l
		imgTest.Pixbuf = new Gdk.Pixbuf("test.jpg");
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		if (timeout != null) timeout.finished = true;
		Application.Quit ();
		a.RetVal = true;
	}

	protected virtual void OnCmdStartClicked (object sender, System.EventArgs e)
	{
		RunTest();
	}
	
    private void TimeoutCallback(object state)
    {
		lblProposition.Text = "Test complete - press Start to begin another";
		test_running = false;
    }	
	
	Thread timeout_thread = null;
	bool timed_out = false;
	ThreadTimeout timeout = null;
	
	private void RunTest()
	{
		timed_out = false;
        //timeout = new ThreadTimeout(new WaitCallback(TimeoutCallback));
        //Thread timeout_thread = new Thread(new ThreadStart(timeout.Execute));
        //timeout_thread.Start();
		
		lblProposition.Text = test.GetProposition();		
		imgTest.Pixbuf = new Gdk.Pixbuf("test.jpg");
		test_running = true;
		Console.WriteLine("Start test");
	}

	protected virtual void OnImgTestButtonPressEvent (object o, Gtk.ButtonPressEventArgs args)
	{
		
	}

	protected virtual void OnExitActionActivated (object sender, System.EventArgs e)
	{
		
		Application.Quit();
	}

	protected virtual void OnMotionNotifyEvent (object o, Gtk.MotionNotifyEventArgs args)
	{
		if (test_running)
		{
			Gdk.ModifierType state;
			Gdk.EventMotion ev = args.Event;
			Gdk.Window window = ev.Window;
			test.StoreMousePosition(ev.X, ev.Y);
			args.RetVal = true;
		}
	}

	protected virtual void OnButtonPressEvent (object o, Gtk.ButtonPressEventArgs args)
	{
		if (test.Clicked())
		{
			test_running = false;
			if ((!timed_out) && (timeout_thread != null))
			{
				//timeout.finished = true;
				timeout_thread.Abort();
			}
			lblProposition.Text = "Test complete - press Start to begin another";
		}
	}

	protected virtual void OnSaveResultsActionActivated (object sender, System.EventArgs e)
	{
		test.ShowAllResults();
	}

}