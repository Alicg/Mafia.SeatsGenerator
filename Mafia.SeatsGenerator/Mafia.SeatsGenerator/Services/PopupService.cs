using System.Collections.Generic;
using System.Linq;
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

        public async Task<T> SelectChoicePopup<T>(List<T> choices, string title) where T : class
        {
            var selectedIndex = await MaterialDialog.Instance.SelectChoiceAsync(title, choices.Select(v => v.ToString()).ToList(), closeOnSelection: true);
            if (selectedIndex != -1)
            {
                return choices[selectedIndex];
            }

            return null;
        }
    }
}