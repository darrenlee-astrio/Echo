# Echo
Echo is a minimal API written in C# .NET 8 that echoes all requests with `/api` in their path.

## Description
Echo is a lightweight and simple API service built using the .NET 8 minimal APIs feature. It provides a straightforward way to echo back any requests that contain `/api` in their path. This can be useful for testing and debugging purposes, or as a simple way to inspect incoming requests.

## Inspiration
I've always found myself needing to create mock APIs to test my own APIs. Whether it's for ensuring that requests are executed successfully or simulating different scenarios, having a mock API handy has been invaluable. With Echo, I've streamlined this process by creating a single API that can handle all my mocking needs. Now, I can simply send requests to Echo and verify their execution without the need for multiple mock APIs. This not only saves time but also makes testing and development more efficient.

## Features
- Minimal API written in C# .NET 8
- Echoes all requests with `/api` in their path
- Easy to use and deploy

## Getting Started
To get started with Echo, simply clone the repository and build the project using .NET CLI or Visual Studio. Then, run the application and start sending requests to it.

``` bash
git clone https://github.com/darrenleeyx/echo.git
cd echo
dotnet build
dotnet run --project src/Echo.Api/Echo.Api.csproj
```

Once the application is running, you can send requests to it using your favorite HTTP client.

## Usage 1 - Echoing request body
Echo listens for incoming requests and echoes back any requests that contain `/api` in their path. You can send requests using any HTTP method (GET, POST, PUT, DELETE, etc.) and include `/api` in the URL path. The request body will be echoed back in the response.

## Example
Request:
``` bash
POST https://echo-tra6.onrender.com/api/echo
Content-Type: application/json
{
  "message": "Hello, Echo!"
}
```
Response:
``` bash
{
  "message": "Hello, Echo!"
}
```

## Usage 2 - Return specified HTTP status code
To set the response to return a specific status code, include the desired HTTP status code in the `x-expected-status-code` request header. Echo will return an empty body for the specified status code. If set to `x-expected-status-code:200`, it will work as defined in usage 1.

## Example
Request:
``` bash
POST https://echo-tra6.onrender.com/api/echo
X-Expected-Status-Code: 400
Content-Type: application/json
{
  "message": "Hello, Echo!"
}
```
Response:
``` bash
400 Bad Request
```

## GitHub Actions
Echo includes a GitHub Actions workflow that automatically builds and publishes artifacts for each commit. You can download the latest artifact by following these steps:

1. Go to [Actions](https://github.com/darrenleeyx/echo/actions).
2. Click on the latest workflow run.
3. Under "Artifacts", click on the artifact you want to download.
4. Click the "Download" button to download the artifact to your local machine.

## Contributing
Contributions are welcome! If you find any bugs or have suggestions for improvement, please open an issue or submit a pull request.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

