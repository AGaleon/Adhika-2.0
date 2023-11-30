using Mopups.Pages;
using Mopups.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Adhika;

public partial class FloatingTextbox : PopupPage
{
	string TextType;
	public FloatingTextbox(string type)
	{
		InitializeComponent();
		TextType = type;
	}
    public static event EventHandler<string> Type;
    public static event EventHandler<string> DataSent;
    public static event EventHandler<bool> GetText;
    private async void Button_Clicked(object sender, EventArgs e)
    {
        DataSent?.Invoke(this, editor.Text);
        Type?.Invoke(this, TextType);
        GetText?.Invoke(this, true);
        await MopupService.Instance.PopAsync();
    }
}//