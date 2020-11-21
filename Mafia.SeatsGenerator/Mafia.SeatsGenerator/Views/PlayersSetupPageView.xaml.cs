using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mafia.SeatsGenerator.Models;
using Mafia.SeatsGenerator.ViewModels;
using Plugin.Iconize;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.Material.Forms.UI;
using XF.Material.Forms.UI.Dialogs;

namespace Mafia.SeatsGenerator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlayersSetupPageView : ContentPage
    {
        public const string EditMenuName = "Редактировать";
        public const string DeleteMenuName = "Удалить";
        private PlayersSetupPageViewModel viewModel;
        
        public PlayersSetupPageView()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (this.BindingContext is PlayersSetupPageViewModel vm)
            {
                this.viewModel = vm;
                ((INotifyCollectionChanged)vm.Players).CollectionChanged += async (sender, args) =>
                {
                    if (((MainPage) this.Parent).CurrentPage != this || args.Action != NotifyCollectionChangedAction.Add)
                    {
                        return;
                    }
                    
                    var lastItem = vm.Players.LastOrDefault();
                    if (lastItem != null)
                    {
                        this.PlayersListView.ScrollTo(lastItem, ScrollToPosition.Center, false);

                        var lastEntry = await Task.Run(() =>
                        {

                            var cells = this.PlayersListView.TemplatedItems.SelectMany(v => v.LogicalChildren);
                            var entries = cells.SelectMany(v => v.LogicalChildren).OfType<Entry>();
                            return entries.LastOrDefault();
                        });
                        
                        await Task.Delay(100);
                        lastEntry?.Focus();
                    }
                };
            }
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
            var uri = new Uri($@"https://the-mafia.net/index.php?q=autocmpl/users/{namePart}");
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

        private void MoreButton_OnMenuSelected(object sender, MenuSelectedEventArgs e)
        {
            if (sender is MaterialMenuButton menuButton && e.Result.Index >= 0)
            {
                var player = menuButton.BindingContext as Player;
                var menuText = this.MoreButtonMenuItems[e.Result.Index].Text;
                switch (menuText)
                {
                    case EditMenuName:
                    {
                        MaterialDialog.Instance.ShowCustomContentAsync(new PlayerInfoView {BindingContext = player}, null, "Информация игрока", dismissiveText:null);
                        break;
                    }
                    case DeleteMenuName:
                    {
                        this.viewModel?.RemovePlayerCommand.Execute(player);
                        break;
                    }
                }
            }
        }
    }
}