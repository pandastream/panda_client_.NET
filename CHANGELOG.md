## 0.1.1 (January 31, 2011)

Features:

    - Added the ServiceProxy property to expose the underlying ServiceProxy object which serves as a proxy for Panda web service requests.
        new VideoService().ServiceProxy.PostJson(...)

    - Added the ability to define a network proxy used when submitting the HTTP request
        new VideoService().SetWebProxyCredentials("host", port, "username", "password");
    
## 0.1.0 (January 21, 2011)

    - First release