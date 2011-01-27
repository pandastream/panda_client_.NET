using System.Collections.Generic;
using System.Web.Script.Serialization;
using Panda.Core;
using Panda.Domain;

namespace Panda.Services
{
    public class VideoService
    {
        #region Private Members
        private readonly IServiceProxy _proxy;
        private Dictionary<string, string> EmptyParameterList
        {
            get
            {
                return new Dictionary<string, string>();
            }
        } 
        #endregion

        #region Ctors
        public VideoService(string cloudId, string accessKey, string secretKey, string apiHost)
        {
            _proxy = new ServiceProxy(cloudId, accessKey, secretKey, apiHost);
            WireDebuggingEvents();
        }

        public VideoService(string cloudId, string accessKey, string secretKey, string apiHost, int apiPort, int apiVersion)
        {
            _proxy = new ServiceProxy(cloudId, accessKey, secretKey, apiHost, apiPort, apiVersion);
            WireDebuggingEvents();
        }

        // additional constructors created to support unit testing
        public VideoService(IServiceProxy _serviceProxy)
        {
            _proxy = _serviceProxy;
            WireDebuggingEvents();
        }

        private void WireDebuggingEvents()
        {
            _proxy.OnDebugEvent += new ServiceProxy.DebugEventHandler(_proxy_OnDebugEvent);
            _proxy.OnWebException += new ServiceProxy.WebExceptionHandler(_proxy_OnWebException);
        } 
        #endregion

        #region Properties
        private IJsonSerializer _serializer;
        public IJsonSerializer JsonSerializer
        {
            get
            {
                if (_serializer == null)
                    _serializer = new JsonSerializer();
                return _serializer;
            }
            set
            {
                _serializer = value;
            }
        }

        /// <summary>
        /// Gets the underlying ServiceProxy object which serves as a proxy for Panda web service requests.
        /// </summary>
        public IServiceProxy ServiceProxy
        {
            get
            {
                return _proxy;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Will create a network proxy assigned to the underlying HTTP request using the supplied
        /// host, port and network credentials.
        /// </summary>
        /// <param name="webProxyHost">The proxy host</param>
        /// <param name="webProxyPort">The proxy port</param>
        /// <param name="username">The user name associated with the network credentials</param>
        /// <param name="password">The password associated with the network credentials</param>
        public void SetWebProxyCredentials(string webProxyHost, int webProxyPort,
            string username, string password)
        {
            _proxy.SetWebProxyCredentials(webProxyHost, webProxyPort, username, password);
        }

        /// <summary>
        /// Returns a list of videos associated with the cloudId and accessKey supplied to the VideoService
        /// </summary>
        /// <returns>A list of videos contained in a particular cloud</returns>
        public IList<Video> GetVideos()
        {
            return JsonSerializer.Deserialize<IList<Video>>(
                _proxy.GetJson("videos.json", EmptyParameterList));
        }

        /// <summary>
        /// Get a video by videoId
        /// </summary>
        /// <returns>A video</returns>
        public Video GetVideo(string videoId)
        {
            return JsonSerializer.Deserialize<Video>(
               _proxy.GetJson(string.Format("videos/{0}.json", videoId), EmptyParameterList));
        }

        /// <summary>
        /// Returns a list of all encodings for a particular video
        /// </summary>
        /// <param name="videoId">The video's Id</param>
        /// <returns>A list of encodings</returns>
        public IList<VideoEncoding> GetVideoEncodings(string videoId)
        {
            return JsonSerializer.Deserialize<IList<VideoEncoding>>(
                _proxy.GetJson(string.Format("videos/{0}/encodings.json", videoId), EmptyParameterList));
        }

        /// <summary>
        /// Uploads a video using the provided video URL
        /// </summary>
        /// <param name="videoUrl">The fully qualified url where the video resides</param>
        /// <returns>The newly uploaded video metadata</returns>
        public Video UploadVideo(string videoUrl)
        {
            var parameters = new Dictionary<string, string>{
                {"source_url", videoUrl}
            };

            return JsonSerializer.Deserialize<Video>(_proxy.PostJson("videos.json", parameters));
        }

        /// <summary>
        /// Uploads the supplied video content
        /// </summary>
        /// <param name="file">The content of the video in bytes</param>
        /// <returns>The newly uploaded video metadata</returns>
        public Video UploadVideo(byte[] file, string fileName)
        {
            return JsonSerializer.Deserialize<Video>(
                _proxy.PostJson("videos.json", EmptyParameterList, file, fileName));
        }

        /// <summary>
        /// Delete a video by videoId
        /// </summary>
        /// <returns>A video</returns>
        public bool DeleteVideo(string videoId)
        {
            var response = _proxy.Delete(string.Format("videos/{0}.json", videoId),
                new Dictionary<string, string>());
            return (response != null) ? response.StatusCode == System.Net.HttpStatusCode.OK : false;
        }

        /// <summary>
        /// Get a cloud by cloudId
        /// </summary>
        /// <returns>A cloud</returns>
        public Cloud GetCloud(string cloudId)
        {
            return JsonSerializer.Deserialize<Cloud>(
                _proxy.GetJson(string.Format("clouds/{0}.json", cloudId),
                new Dictionary<string, string>()));
        } 
        #endregion

        #region Debugging Events

        public delegate void DebugEventHandler(string title, string message);
        public event DebugEventHandler OnDebugEvent;

        public delegate void WebExceptionHandler(System.Net.WebException exception);
        public event WebExceptionHandler OnWebException;

        void _proxy_OnDebugEvent(string title, string message)
        {
            var handler = OnDebugEvent;
            if (handler != null) handler(title, message);
        }

        void _proxy_OnWebException(System.Net.WebException exception)
        {
            var handler = OnWebException;
            if (handler != null) handler(exception);
        }

        #endregion
    }
}