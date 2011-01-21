using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Panda.Core;
using Panda.Services;

namespace Panda.Tests
{
    [TestFixture]
    public class VideoServiceTests
    {
        private IServiceProxy _pandaServiceProxy;
        private IJsonSerializer _serializer;

        private VideoService GetVideoService()
        {
            _pandaServiceProxy = MockRepository.GenerateStub<IServiceProxy>();
            _serializer = MockRepository.GenerateStub<IJsonSerializer>();
            var videoService = new VideoService(_pandaServiceProxy);
            videoService.JsonSerializer = _serializer;
            return videoService;
        }

        [Test]
        public void GetVideos_WhenCalled_CallsProxyMethodWithCorrectParameters()
        {
            var videoService = GetVideoService();

            videoService.GetVideos();

            _pandaServiceProxy.AssertWasCalled(proxy => 
                proxy.GetJson("videos.json", new Dictionary<string, string>()));
        }

        [Test]
        public void GetVideos_WhenCalled_CallsSerializer()
        {
            var videoService = GetVideoService();

            videoService.GetVideos();

            _serializer.AssertWasCalled(serializer =>
                serializer.Deserialize<IList<Panda.Domain.Video>>(Arg<string>.Is.Anything));
        }

        [Test]
        public void GetVideo_VideoIdSupplied_CallsProxyMethodWithCorrectParameters()
        {
            var videoService = GetVideoService();
            var videoId = "some_video_id";

            videoService.GetVideo(videoId);

            _pandaServiceProxy.AssertWasCalled(proxy =>
                proxy.GetJson(string.Format("videos/{0}.json", videoId), new Dictionary<string, string>()));
        }

        [Test]
        public void GetVideo_VideoIdSupplied_CallsSerializer()
        {
            var videoService = GetVideoService();
            var videoId = "some_video_id";

            videoService.GetVideo(videoId);

            _serializer.AssertWasCalled(serializer =>
                serializer.Deserialize<Panda.Domain.Video>(Arg<string>.Is.Anything));
        }

        [Test]
        public void GetVideoEncodings_VideoIdSupplied_CallsProxyMethodWithCorrectParameters()
        {
            var videoService = GetVideoService();
            var videoId = "some_video_id";

            videoService.GetVideoEncodings(videoId);

            _pandaServiceProxy.AssertWasCalled(proxy =>
                proxy.GetJson(string.Format("videos/{0}/encodings.json", videoId), new Dictionary<string, string>()));
        }

        [Test]
        public void GetVideoEncodings_VideoIdSupplied_CallsSerializer()
        {
            var videoService = GetVideoService();
            var videoId = "some_video_id";

            videoService.GetVideoEncodings(videoId);

            _serializer.AssertWasCalled(serializer =>
                serializer.Deserialize<IList<Panda.Domain.VideoEncoding>>(Arg<string>.Is.Anything));
        }

        [Test]
        public void UploadVideo_VideoUrlSupplied_CallsProxyMethodWithCorrectParameters()
        {
            var videoService = GetVideoService();
            var videoUrl = "http://www.google.com/some_video.avi";

            videoService.UploadVideo(videoUrl);

            _pandaServiceProxy.AssertWasCalled(proxy =>
                proxy.PostJson("videos.json", new Dictionary<string, string> { { "source_url", videoUrl } }));
        }

        [Test]
        public void UploadVideo_VideoUrlSupplied_CallsSerializer()
        {
            var videoService = GetVideoService();
            var videoUrl = "http://www.google.com/some_video.avi";

            videoService.UploadVideo(videoUrl);

            _serializer.AssertWasCalled(serializer =>
                serializer.Deserialize<Panda.Domain.Video>(Arg<string>.Is.Anything));
        }

        [Test]
        public void UploadVideo_FileDataSupplied_CallsProxyMethodWithCorrectParameters()
        {
            var videoService = GetVideoService();
            var videoContent = new byte[8];
            var videoFileName = "some_video.avi";

            videoService.UploadVideo(videoContent, videoFileName);

            _pandaServiceProxy.AssertWasCalled(proxy =>
                proxy.PostJson("videos.json", new Dictionary<string, string>(), videoContent, videoFileName));
        }

        [Test]
        public void UploadVideo_FileDataSupplied_CallsSerializer()
        {
            var videoService = GetVideoService();
            var videoContent = new byte[8];
            var videoFileName = "some_video.avi";

            videoService.UploadVideo(videoContent, videoFileName);

            _serializer.AssertWasCalled(serializer =>
                serializer.Deserialize<Panda.Domain.Video>(Arg<string>.Is.Anything));
        }

        [Test]
        public void DeleteVideo_VideoIdSupplied_CallsProxyMethodWithCorrectParameters()
        {
            var videoService = GetVideoService();
            var videoId = "some_video_id";

            videoService.DeleteVideo(videoId);

            _pandaServiceProxy.AssertWasCalled(proxy =>
                proxy.Delete(string.Format("videos/{0}.json", videoId),
                new Dictionary<string, string>()));
        }

        [Test]
        public void GetCloud_CloudIdSupplied_CallsProxyMethodWithCorrectParameters()
        {
            var videoService = GetVideoService();
            var cloudId = "some_cloud_id";

            videoService.GetCloud(cloudId);

            _pandaServiceProxy.AssertWasCalled(proxy =>
                proxy.GetJson(string.Format("clouds/{0}.json", cloudId), new Dictionary<string, string>()));
        }

        [Test]
        public void GetCloud_CloudIdSupplied_CallsSerializer()
        {
            var videoService = GetVideoService();
            var cloudId = "some_cloud_id";

            videoService.GetCloud(cloudId);

            _serializer.AssertWasCalled(serializer =>
                serializer.Deserialize<Panda.Domain.Cloud>(Arg<string>.Is.Anything));
        }
    }
}
