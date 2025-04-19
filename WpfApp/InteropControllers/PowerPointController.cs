using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using System.Runtime.InteropServices;
using System.Windows;

public class PowerPointController
{
    private PowerPoint.Application _pptApp;

    public PowerPointController()
    {
        try
        {
            _pptApp = (PowerPoint.Application)ComInterop.GetActiveObject("PowerPoint.Application");
        }
        catch (COMException)
        {
            _pptApp = new PowerPoint.Application();
        }
    }

    public void StartPresentation()
    {
        if (_pptApp.Presentations.Count == 0)
        {
            MessageBox.Show("No PowerPoint presentation is currently open.", "PowerPoint Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var pres = _pptApp.ActivePresentation;
        pres.SlideShowSettings.Run();
    }

    public void NextSlide()
    {
        if (!IsSlideshowRunning()) return;

        _pptApp.SlideShowWindows[1].View.Next();
    }

    public void PreviousSlide()
    {
        if (!IsSlideshowRunning()) return;

        _pptApp.SlideShowWindows[1].View.Previous();
    }

    private bool IsSlideshowRunning()
    {
        if (_pptApp.SlideShowWindows.Count == 0)
        {
            MessageBox.Show("No slideshow is currently running.", "PowerPoint Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        return true;
    }
}
