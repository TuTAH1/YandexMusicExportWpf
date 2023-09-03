using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Titanium;
using static Titanium.TypesFuncs;
using CheckBox = System.Windows.Controls.CheckBox;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace YandexMusicExportWpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public enum CurrentState
		{
			Initial, //default state
			PlaylistsLoaded, //playlists loaded
		}

		private CheckBox CheckAll = new CheckBox();

		private CurrentState state = CurrentState.Initial;
		public CurrentState	State	{
			get => state;
			set
			{
				state = value;
				switch (state)
				{
					case CurrentState.Initial:
						tbUsername.BorderBrush = new SolidColorBrush(Colors.Gray); //rgb(171, 173, 179)? 
						tblUsername.Text = "Имя пользователя (адрес электронной почты)";
						btnSubmit.Content = "Получить плейлисты";
						imgUsernameCross.Visibility = Visibility.Collapsed;
						tbUsername.IsEnabled = true;
						spCheckBox.Children.Clear();  //remove all elements from spCheckBox
						break;
					case CurrentState.PlaylistsLoaded:
						tbUsername.IsEnabled = false;
						imgUsernameCross.Visibility = Visibility.Visible;
						imgUsernameLoading.Visibility = Visibility.Collapsed;
						btnSubmit.Content = "Экспортировать";
						break;
				}
			}
		}

		UserPlaylistsResponse? UserPlaylists = null;

		private string Username;

		public MainWindow()
		{
			InitializeComponent();
			State = CurrentState.Initial;
			CheckAll.Content = "Выбрать все";
			CheckAll.Checked += CheckedChanged;
			CheckAll.Unchecked += CheckedChanged;

			void CheckedChanged (object sender, RoutedEventArgs e)
			{
				var cb = sender as CheckBox;

				foreach (CheckBox cbPlaylist in spCheckBox.Children)
				{
					cbPlaylist.IsChecked = cb.IsChecked;
				}
			};
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void TbUsername_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			State = CurrentState.Initial;
		}

		private async Task<UserPlaylistsResponse?> LoadUserPlaylists(string Username)
		{
			try
			{
				using HttpClient client = new HttpClient();
				string url = $"https://music.yandex.ru/handlers/playlists.jsx?owner={Username}";

				HttpResponseMessage response = await client.GetAsync(url);
				response.EnsureSuccessStatusCode();

				string responseContent = await response.Content.ReadAsStringAsync();

				var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
				return JsonSerializer.Deserialize<UserPlaylistsResponse>(responseContent, options);
			}
			catch (Exception e)
			{
				imgUsernameLoading.Visibility = Visibility.Collapsed;
				e.ShowMessageBox();
				return null;
			}
		}

		private async void btnSubmit_Click(object sender, RoutedEventArgs e)
		{
			btnSubmit.IsEnabled = false;
			var temp = imgUsernameCross.Visibility;
			imgUsernameCross.Visibility = Visibility.Hidden;
			imgUsernameLoading.Visibility = Visibility.Visible;

			btnSubmit_ClickAsync();

			imgUsernameCross.Visibility = temp;
			imgUsernameLoading.Visibility = Visibility.Hidden;
			btnSubmit.IsEnabled = true;
		}

		private async Task btnSubmit_ClickAsync()
		{
			switch (State)
			{
				case CurrentState.Initial:
					try
					{
						Username = tbUsername.Text = tbUsername.Text.Slice(0,"@", AlwaysReturnString: true);
						if (string.IsNullOrWhiteSpace(Username))
						{
							tbUsername.BorderBrush = Brushes.Red;
							return;
						}

						imgUsernameLoading.Visibility = Visibility.Visible;
						UserPlaylists = await LoadUserPlaylists(Username);

						if (UserPlaylists == null)
						{
							imgUsernameLoading.Visibility = Visibility.Collapsed;
							return;
						}


						spCheckBox.Children.Add(CheckAll); // add "Check all" checkbox
						//:Adding playlist to the menu
						foreach (var playlist in UserPlaylists!.Playlists)
						{
							CheckBox checkBox = new()
							{
								Content = $"{playlist.Title}, {playlist.TrackCount} треков",
								Tag = playlist.Kind,
								Foreground = new SolidColorBrush(Color.FromRgb(70,70,70)),
							};

							void ChangeChecked (object sender, RoutedEventArgs args)  
							{
								var that = (CheckBox)sender;
								that.Foreground = that.IsChecked == true? Brushes.Black : new SolidColorBrush(Color.FromRgb(70,70,70));
							};

							checkBox.Checked += ChangeChecked;
							checkBox.Unchecked += ChangeChecked;
						
							spCheckBox.Children.Add(checkBox);
						}

						tbUsername.BorderBrush = Brushes.Green;

						State = CurrentState.PlaylistsLoaded;
					}
					catch (Exception exception)
					{
						imgUsernameLoading.Visibility = Visibility.Collapsed;
						exception.ShowMessageBox();
					}
					break;


				case CurrentState.PlaylistsLoaded:
					//if there's no any checked playlist, return
                    bool noCheckedPlaylists = spCheckBox.Children.Cast<CheckBox>().All(cbPlaylist => cbPlaylist.IsChecked != true);
					if (noCheckedPlaylists) return;

					//Open WPF save folder dialog 
                    var folderBrowser = new FolderBrowserDialog() {
						ShowNewFolderButton = true
					};
					var result = folderBrowser.ShowDialog();
					if (result != System.Windows.Forms.DialogResult.OK) return;


					//Iterating over each checked playlist
					foreach (CheckBox cbPlaylist in spCheckBox.Children)
					{
						if (cbPlaylist.IsChecked == false) {continue;}

						try
						{
							using HttpClient client = new HttpClient();
							string url = $"https://music.yandex.ru/handlers/playlist.jsx?owner={tbUsername.Text}&kinds={cbPlaylist.Tag}";

							HttpResponseMessage response = await client.GetAsync(url);
							response.EnsureSuccessStatusCode();

							string responseContent = await response.Content.ReadAsStringAsync();

							var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };


							cbPlaylist.Foreground = Brushes.Orange;
							
							var playlistResponse = JsonSerializer.Deserialize<PlaylistResponse>(responseContent, options);

							// Извлечение названия плейлиста и списка треков из полученного ответа
							var playlistTitle = playlistResponse?.Playlist.Title;
							var tracks = playlistResponse?.Playlist.Tracks;
							string allFile = "";

							// Итерация по каждому треку в списке треков
							foreach (var track in tracks!)
							{
								var artistsNames = "";

								// Итерация по каждому артисту в списке артистов трека
								foreach (var artist in track.Artists) artistsNames += artist.Name + ", ";

								// Удаление последней запятой из строки артистов
								artistsNames = artistsNames.TrimEnd(',', ' ');

								// Формирование строки вида "Имя артиста - Название трека"
								var fullTrack = artistsNames + " - " + track.Title + "\n";

								// Добавление строки трека к переменной AllFile
								allFile += fullTrack;
							}

							try
							{
								await using var fs = new StreamWriter($"{folderBrowser.SelectedPath}\\{playlistTitle}.txt");
								await fs.WriteAsync(allFile);
								fs.Close();
							}
							catch (Exception e) 
							{
								if (e is DirectoryNotFoundException or IOException)
								{
									try
									{
										await using var fs = new StreamWriter($"{folderBrowser.SelectedPath}\\Некорректное имя{cbPlaylist.Tag}.txt");
										await fs.WriteAsync(allFile);
										fs.Close();
									}
									catch (Exception ex) 
									{
										ex.ShowMessageBox();
									}
								}
								else
									e.ShowMessageBox();
							}
							

							cbPlaylist.Foreground = Brushes.Green;
						}
						catch (Exception exception)
						{
							cbPlaylist.Foreground = Brushes.Red;
							//Add error to the tooltip of checkbox
							ToolTipService.SetShowDuration(cbPlaylist, 5000);
							ToolTipService.SetToolTip(cbPlaylist, exception.Message);

						}

						
						// Сохранение файла

					}
					break;
			}
		}

		//: Allow only latin and @. symbols
		private void TbUsername_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			bool isValidInput = IsLatinCharacter(e.Text) || IsAllowedSymbol(e.Text);
			
			if (!isValidInput)
			{
				e.Handled = true;
			}
		}

		private bool IsLatinCharacter(string input) => input.All(c => c is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9');
		private bool IsAllowedSymbol(string input) => input == "." || input == "@";

		//: Forbid space
		private void TbUsername_OnPreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space)
			{
				e.Handled = true;
			}

			if (e.Key == Key.Enter)
			{
				btnSubmit_Click(sender, new RoutedEventArgs());
			}
		}

		private void ImgUsernameCross_OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			State = CurrentState.Initial;
			tbUsername.IsEnabled = true;
			tbUsername.Text = "";
			imgUsernameLoading.Visibility = Visibility.Collapsed;
		}
	}

	internal class UserPlaylistsResponse
	{
		public  PlaylistData[] Playlists{ get; set; }
	}

	internal class PlaylistResponse
	{
		public PlaylistData Playlist { get; set; }
	}

	internal class PlaylistData
	{
		public string Title { get; set; }
		public Track[]? Tracks { get; set; }
		public int Kind { get; set; }
		public int TrackCount { get; set; }
	}

	internal class Track
	{
		public string Title { get; set; }
		public Artist[] Artists { get; set; }
	}

	internal class Artist
	{
		public string Name { get; set; }
	}
}
