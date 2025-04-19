using Word = Microsoft.Office.Interop.Word;
using System.Runtime.InteropServices;
using System.Windows;

public class WordController
{
    private Word.Application _wordApp;

    public WordController()
    {
        try
        {
            _wordApp = (Word.Application)ComInterop.GetActiveObject("Word.Application");
        }
        catch (COMException)
        {
            _wordApp = new Word.Application();
        }
    }

    public void ScrollDown()
    {
        if (!IsDocumentOpen()) return;

        _wordApp.ActiveWindow.SmallScroll(Down: 3);
    }

    public void ScrollUp()
    {
        if (!IsDocumentOpen()) return;

        _wordApp.ActiveWindow.SmallScroll(Up: 3);
    }

    private bool IsDocumentOpen()
    {
        if (_wordApp.Documents.Count == 0)
        {
            MessageBox.Show("No Word document is currently open.", "Word Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        return true;
    }
}
