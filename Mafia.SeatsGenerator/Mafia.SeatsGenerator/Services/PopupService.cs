using XF.Material.Forms.UI.Dialogs;

namespace Mafia.SeatsGenerator.Services
{
    public class PopupService
    {
        public void ShowAlert(string alertMessage, string title)
        {
            MaterialDialog.Instance.AlertAsync(alertMessage, title);
        }
    }
}