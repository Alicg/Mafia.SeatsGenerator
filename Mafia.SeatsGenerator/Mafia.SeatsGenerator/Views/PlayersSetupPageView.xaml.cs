using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mafia.SeatsGenerator.Models;
using Plugin.Iconize;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.Material.Forms.UI.Dialogs;
using XF.Material.Forms.UI.Dialogs.Configurations;

namespace Mafia.SeatsGenerator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlayersSetupPageView : ContentPage
    {
        public PlayersSetupPageView()
        {
            InitializeComponent();
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            if (sender is IconButton button)
            {
                var player = button.BindingContext as Player;
                if (player == null || string.IsNullOrEmpty(player.Name) || player.Name.Length <= 1)
                {
                    return;
                }

                List<string> names;
                
                using(await MaterialDialog.Instance.LoadingDialogAsync(message: "Ищу игроков на the-mafia.net..."))
                {
                    names = await this.SelectPlayerNameOptions(player.Name);
                }
                
                var selectedIndex = await MaterialDialog.Instance.SelectChoiceAsync("Выберите игрока", names, closeOnSelection: true);
                if (selectedIndex >= 0)
                {
                    player.Name = names[selectedIndex];
                }
            }
        }
        
        private async Task<List<string>> SelectPlayerNameOptions(string namePart)
        {
            var retList = new List<string>();
            var uri = new Uri ($@"https://the-mafia.net/index.php?q=autocmpl/users/{namePart}");
            var client = new HttpClient();
            try
            {
                HttpResponseMessage response;
                response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var pattern = @"""(?<name>.*?)\(.*?";
                    var strings = content.Split(',');
                    foreach (var s in strings)
                    {
                        var nameMatch = Regex.Match(s, pattern);
                        if (nameMatch.Success && nameMatch.Groups.Count > 0)
                        {
                            var name = nameMatch.Groups["name"].Value;
                            if (name.Contains("\\u"))
                            {
                                name = FromStupidUnicodeFormat(name);
                            }

                            if (!retList.Contains(name))
                            {
                                retList.Add(name);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return retList;
        }

        private static string FromStupidUnicodeFormat(string unicodeCharacters)
        {
            var pattern = @"(?<=\\u)(?<code>.*?)(\\u|$)";
            var codeMatches = Regex.Matches(unicodeCharacters, pattern);
            var byteString = new List<byte>();
            foreach (Match codeMatch in codeMatches)
            {
                var hexCode = codeMatch.Groups["code"].Value;
                if (short.TryParse(hexCode, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var intCode))
                {
                    var bytesCode = BitConverter.GetBytes(intCode);
                    byteString.AddRange(bytesCode);
                }
            }

            return Encoding.Unicode.GetString(byteString.ToArray());
        }
    }
}