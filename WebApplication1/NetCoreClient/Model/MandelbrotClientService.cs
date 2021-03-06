﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NetworkLibraryMandelBrot;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ClientApp
{
    public class MandelBrotClientService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        public MandelBrotClientService(HttpClient httpClient, ILogger<MandelBrotClientService> logger)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<Bitmap> GetMandelbrotBitmap(MandelBrotRequest request)
        {

            string json = JsonConvert.SerializeObject(request);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("api/MandelBrot", content);
            resp.EnsureSuccessStatusCode();
            string newJson = await resp.Content.ReadAsStringAsync();
            var answer = JsonConvert.DeserializeObject<MandelbrotBitMapAnswer>(newJson);
            Bitmap c = null;
            using (MemoryStream ms = new MemoryStream(answer.Picture))
            {
                BinaryFormatter bf = new BinaryFormatter();
                c = (Bitmap)bf.Deserialize(ms);
            }
            return c;
        }
    }
}
