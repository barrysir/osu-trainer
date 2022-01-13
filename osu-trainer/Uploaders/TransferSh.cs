using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace osu_trainer.Uploaders
{
    class TransferSh
    {
        private HttpResponseMessage response;
        private string mainLink;
        private string downloadLink;

        public TransferSh(HttpResponseMessage response)
        {
            this.response = response;

            string rawlink = this.response.Content.ReadAsStringAsync().Result;
            this.mainLink = rawlink;

            // https://transfer.sh/2gKpIc/file.txt
            // turn into https://transfer.sh/get/2gKpIc/file.txt
            var builder = new UriBuilder(new Uri(rawlink));
            Debug.Assert(builder.Path.StartsWith("/"));
            builder.Path = "/get" + builder.Path;
            this.downloadLink = builder.Uri.AbsoluteUri.ToString();
        }

        public string MainLink => mainLink;
        public string DownloadLink => downloadLink;
        public string DeletionLink => this.response.Headers.GetValues("X-Url-Delete").First();
    }

    class TransferShFactory {
        private HttpClient httpClient = new HttpClient();
        private int maxDays = 0;
        private int maxDownloads = 0;

        public int MaxDays { get => maxDays; set => maxDays = value; }
        public int MaxDownloads { get => maxDownloads; set => maxDownloads = value; }

        public TransferShFactory(int maxDays=0, int maxDownloads=0)
        {
            MaxDays = maxDays;
            MaxDownloads = maxDownloads;
        }

        public async Task<TransferSh> Upload(FileStream fs, string filename)
        {
            MultipartFormDataContent form = new MultipartFormDataContent();

            string url = $"https://transfer.sh/{filename}";

            form.Add(new StreamContent(fs), "file", filename);
            if (this.MaxDays != 0)
            {
                form.Headers.Add("Max-Days", this.MaxDays.ToString());
            }
            if (this.MaxDownloads != 0)
            {
                form.Headers.Add("Max-Downloads", this.MaxDownloads.ToString());
            }
            HttpResponseMessage response = await httpClient.PutAsync(url, form);
            response.EnsureSuccessStatusCode();     // ??
            return new TransferSh(response);
        }

        public async Task<TransferSh> Upload(string filepath)
        {
            string filename = Path.GetFileName(filepath);
            using (var filestream = new FileStream(filepath, FileMode.Open))
            {
                return await Upload(filestream, filename);
            }
        }
    }
}
