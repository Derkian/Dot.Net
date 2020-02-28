using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Google.Api.YouTubePlayListDaily
{
    class Program
    {
        private const int QUANTIDADE_VIDEOS = 10;
        private const string PARTS = "snippet,status";

        static void Main(string[] args)
        {
            Console.WriteLine("YouTube Data API: Playlist Updates");
            Console.WriteLine("==================================");

            try
            {
                new Program().Run().Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private async Task Run()
        {
            #region SEARCH
            //SEARCH VIDEO
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "REPLACE_ME",
                ApplicationName = this.GetType().ToString()
            });

            foreach (var tag in new string[] { "Sergio Moro", "Coronavirus", "Bolsonaro" })
            {
                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Q = tag; // Replace with your search term.
                searchListRequest.Order = SearchResource.ListRequest.OrderEnum.ViewCount;
                searchListRequest.PublishedAfter = DateTime.Now.AddDays(-1).Date;
                searchListRequest.MaxResults = QUANTIDADE_VIDEOS;

                // Call the search.list method to retrieve results matching the specified query term.
                var searchListResponse = await searchListRequest.ExecuteAsync();

                var playList = await CreatePlayList();

                // Add each result to the appropriate list, and then display the lists of
                // matching videos, channels, and playlists.
                foreach (var searchResult in searchListResponse.Items)
                {
                    switch (searchResult.Id.Kind)
                    {
                        case "youtube#video":
                            await addVideoPlayList(searchResult, playList);
                            break;
                    }
                }
            }
            #endregion
        }

        private async Task<YouTubeService> CreateCredential()
        {
            UserCredential credential;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    // This OAuth 2.0 access scope allows for full read/write access to the
                    // authenticated user's account.
                    new[] { YouTubeService.Scope.Youtube },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(this.GetType().ToString())
                );
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.GetType().ToString()
            });

            return youtubeService;
        }

        private async Task<Playlist> CreatePlayList()
        {
            var youtubeService = await CreateCredential();

            Playlist newPlaylist = null;
            var request = youtubeService.Playlists.List(PARTS);
            request.Mine = true;

            var result = await request.ExecuteAsync();

            foreach (var playlist in result.Items)
            {
                if (playlist.Snippet.PublishedAt.Value.Date <= DateTime.Now.AddDays(-5).Date)
                {
                    await youtubeService.Playlists.Delete(playlist.Id).ExecuteAsync();
                }
                else if (playlist.Snippet.PublishedAt.Value.Date == DateTime.Now.Date)
                {
                    newPlaylist = playlist;
                }
            }

            if (newPlaylist == null)
            {
                // Create a new, private playlist in the authorized user's channel.
                newPlaylist.Snippet = new PlaylistSnippet();
                newPlaylist.Snippet.Title = string.Format("Diário {0}", DateTime.Now.ToString("dd/MM/yyyy"));
                newPlaylist.Snippet.Description = string.Format("Playlist Diário {0}", DateTime.Now.ToString("dd/MM/yyyy"));
                newPlaylist.Status = new PlaylistStatus();
                newPlaylist.Status.PrivacyStatus = "public";
                newPlaylist = await youtubeService.Playlists.Insert(newPlaylist, PARTS).ExecuteAsync();
            }

            return newPlaylist;
        }

        private async Task addVideoPlayList(SearchResult searchResult, Playlist playlist)
        {
            var youtubeService = await CreateCredential();

            // Add a video to the newly created playlist.
            var newPlaylistItem = new PlaylistItem();
            newPlaylistItem.Snippet = new PlaylistItemSnippet();
            newPlaylistItem.Snippet.PlaylistId = playlist.Id;
            newPlaylistItem.Snippet.ResourceId = new ResourceId();
            newPlaylistItem.Snippet.ResourceId.Kind = searchResult.Id.Kind;
            newPlaylistItem.Snippet.ResourceId.VideoId = searchResult.Id.VideoId;
            newPlaylistItem = await youtubeService.PlaylistItems.Insert(newPlaylistItem, "snippet").ExecuteAsync();

            //Console.WriteLine("Playlist item id {0} was added to playlist id {1}.", newPlaylistItem.Id, playlist.Id);
        }
    }
}
