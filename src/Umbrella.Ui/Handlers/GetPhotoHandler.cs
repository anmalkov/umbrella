using MediatR;
using System.Reflection;
using Umbrella.Core.Repositories;
using Umbrella.Core.Services;
using Umbrella.Ui.Requests;

namespace Umbrella.Ui.Handlers;

public class GetPhotoHandler : IRequestHandler<GetPhotoRequest, IResult>
{
    private readonly string _photosDirectory;

    public GetPhotoHandler()
    {
        var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".";
        _photosDirectory = Path.Combine(currentDirectory, "photos");
    }

    public async Task<IResult> Handle(GetPhotoRequest request, CancellationToken cancellationToken)
    {
        var mimeType = "image/jpg";
        var path = GetRandomFileName();
        return path is null ? Results.NotFound() : Results.File(path, contentType: mimeType);
    }

    private string? GetRandomFileName()
    {
        if (!Directory.Exists(_photosDirectory))
        {
            return null;
        }

        var files = Directory.GetFiles(_photosDirectory);
        return files.Length > 0 ? files[new Random().Next(0, files.Length)] : null;
    }
}
