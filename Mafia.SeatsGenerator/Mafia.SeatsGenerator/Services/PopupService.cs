using System.Threading.Tasks;
using XF.Material.Forms.UI.Dialogs;

namespace Mafia.SeatsGenerator.Services
{
    public class PopupService
    {
        public void ShowAlert(string alertMessage, string title)
        {
            MaterialDialog.Instance.AlertAsync(alertMessage, title);
        }

        public async Task<bool?> ConfirmationPopup(string confirmMessage, string title)
        {
            return await MaterialDialog.Instance.ConfirmAsync(confirmMessage, title);
        }
    }
}