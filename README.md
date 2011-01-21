Panda client, .NET
=================


This .NET library provides an interface to the REST API of [Panda](http://pandastream.com), the online video encoding service.

Setup
-----

    var videoService = new Panda.Services.VideoService("cloudId", "accessKey", "secretKey", "apiHost");

Now you can use this instance to interact with your Panda cloud.

Basics
------
 
Returns a list of videos associated with the cloudId and accessKey supplied to the VideoService
 
    IList<Panda.Domain.Video> videos = videoService.GetVideos();
 
Get a video by videoId
 
    Panda.Domain.Video video = videoService.GetVideo("videoId");
 

Returns a list of all encodings for a particular video
 
    IList<Panda.Domain.VideoEncoding> videoEncodings = videoService.GetVideoEncodings("videoId");
 
Upload a video using the video URL
 
    Panda.Domain.Video video = videoService.UploadVideo("http://www.yoursite.com/yourvideo.avi");
 
Upload a video supplying the video file content (as a byte array) and file name
 
    Panda.Domain.Video video = videoService.UploadVideo(fileContents, "fileName");
 
Delete a video by videoId
 
    videoService.DeleteVideo("videoId")
 
Get a cloud by cloudId
 
    Panda.Domain.Cloud cloud = videoService.GetCloud("cloudId");
    
    
This library has been written by Mark Bonano (Applications Architect, Chazmar Solutions LLC). Thanks to him for the great work.
If some functionalities are missing, please fork the project!!
