using CefSharp;
using CefSharp.Wpf;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace Global_Cinema
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window, INotifyPropertyChanged
    {


        public MainWindow()
        {
            InitializeComponent();
        }

        public ObservableCollection<Movies> MoviesList { get; set; }

        private Movies _movie;

        public Movies Movie { get { return _movie; } set { _movie = value; OnpropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnpropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }



        public dynamic Data { get; set; }

        HttpClient HttpClient = new HttpClient();
        HttpResponseMessage responseMessage = new HttpResponseMessage();


        public string videotitleID = "Thor 2011";

            ObservableCollection<Movies> addMoviesList = new ObservableCollection<Movies>();
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
          
   
            
                var name = SearchTextBox.Text;
                responseMessage = HttpClient.GetAsync($@"http://www.omdbapi.com/?plot=full&apikey=30169e52&s={name}").Result;
                var str = responseMessage.Content.ReadAsStringAsync().Result;

                Data = JsonConvert.DeserializeObject(str);



          




            try
            {
                if (Data.Search != null)
                {

                    foreach (var movieLoop in Data.Search)
                    {


                        responseMessage = HttpClient.GetAsync($@"http://www.omdbapi.com/?plot=full&apikey=30169e52&i={movieLoop.imdbID}").Result;
                        var str2 = responseMessage.Content.ReadAsStringAsync().Result;

                        dynamic Data2 = JsonConvert.DeserializeObject(str2);

                        addMoviesList.Add(new Movies()
                        {

                            Year = $" Year: {Data2.Year}",
                            Title = $" {Data2.Title}",
                            Poster = Data2.Poster,
                            imdbRating = $" imdbRating: {Data2.imdbRating}",
                            Genre = $" Genre: {Data2.Genre}",
                            Plot = Data2.Plot,
                        });

                        listbox1.ItemsSource = addMoviesList;



                     


                 




                    }
                }

                else 
                {
                    MessageBox.Show($" There is not  \"{name}\"  in OMDb API");
                }

                
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }




        }



        private void listbox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show($"{addMoviesList[listbox1.SelectedIndex].Title}");
            Search(addMoviesList[listbox1.SelectedIndex].Title, addMoviesList[listbox1.SelectedIndex].Year).GetAwaiter();
        }


        private string _videoId;
        private async Task Search(string movieName, string year)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyA51Ttqhd9_nkWVVpJWwXvmDMIIvINOPCM",
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = $"{movieName} {year} Official Trailer" ; // Replace with your search term.
            searchListRequest.MaxResults = 1;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            List<string> videos = new List<string>();
            List<string> channels = new List<string>();
            List<string> playlists = new List<string>();

            string v = "";

            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            foreach (var searchResult in searchListResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        //   videos.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.VideoId));
                        videos.Add(searchResult.Id.VideoId);
                        break;

                    case "youtube#channel":
                        channels.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
                        break;

                    case "youtube#playlist":
                        playlists.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
                        break;
                }
            }

                        v = videos[0];
         

            MessageBox.Show($" \n www./youtu.be/{v}");



            if(ChromiumBrowser.Address == null)
            {

                Stackyoutubewb.Visibility = Visibility.Visible;
            ChromiumBrowser.Address = $@"https://www.youtube.com/embed/{v}";
            }
            

            if (ChromiumBrowser.Address != null)
            {
               
                ChromiumBrowser.Address = string.Empty;
                ChromiumBrowser.Reload();
                ChromiumBrowser.Address = $@"https://www.youtube.com/embed/{v}";






            }

         
            
        
            


            //Console.WriteLine(String.Format("Videos:\n{0}\n", string.Join("\n", videos)));
            //Console.WriteLine(String.Format("Channels:\n{0}\n", string.Join("\n", channels)));
            //Console.WriteLine(String.Format("Playlists:\n{0}\n", string.Join("\n", playlists)));
            // Console.WriteLine($"{videos.Snippet.Title} (https://www.youtube.com/watch?v={v})");

        }
    }
}
