﻿using Microsoft.Extensions.DependencyInjection;
using Flvt.Application;
using Flvt.Application.Abstractions;
using Flvt.Domain.Photos;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure;
using MediatR;
using Microsoft.Extensions.Configuration;
using Amazon.S3.Model;
using Newtonsoft.Json;
using System.Text;
using Amazon;
using Flvt.Domain;
using Amazon.Runtime;
using HtmlAgilityPack;

namespace Flvt.CLI;

internal class Program
{

    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .Build();

        var serviceProvider = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddScoped<IService, Service>()
            .AddApplication()
            .AddInfrastructure()
            .BuildServiceProvider();

        var s = serviceProvider.GetRequiredService<IService>();

        await s.Run();
    }
}

public class Service : IService
{
    private readonly ISender _sender;
    private readonly IProcessedAdvertisementRepository _repository;
    private readonly IScrapedAdvertisementRepository _scrapedAdvertisementRepository;
    private readonly IAdvertisementPhotosRepository _advertisementPhotosRepository;
    private readonly IEmailService _emailService;
    private readonly IQueuePublisher _queuePublisher;

    public Service(
        ISender sender,
        IProcessedAdvertisementRepository repository,
        IScrapedAdvertisementRepository scrapedAdvertisementRepository,
        IAdvertisementPhotosRepository advertisementPhotosRepository,
        IEmailService emailService,
        IQueuePublisher queuePublisher)
    {
        _sender = sender;
        _repository = repository;
        _scrapedAdvertisementRepository = scrapedAdvertisementRepository;
        _advertisementPhotosRepository = advertisementPhotosRepository;
        _emailService = emailService;
        _queuePublisher = queuePublisher;
    }

    public async Task Run()
    {
        //var a = _repository.GetByFilterAsync(
        //    Subscriber.Register(
        //            "a@a.com",
        //            "PL")
        //        .Amount.AddBasicFilter(
        //            "nname",
        //            "Warszawa",
        //            null,
        //            null,
        //            null,
        //            null,
        //            null,
        //            null)
        //        .Amount);
        //var cmd = new ScrapeAdvertisementsCommand();
        //var cmd = new StartProcessingAdvertisementsCommand();
        //var cmd = new CheckProcessingStatusCommand();
        //var cmd = new EndProcessingCommand();
        //var cmd = new ProcessAdvertisementsCommand();
        //var cmd = new RemoveOutdatedAdvertisementsCommand();
        //var cmd = new RegisterCommand(); // TODO REMOVE

        //var response = await _sender.Send(cmd);

        //var adsR = await _repository.GetAllAsync();
        //var ads = adsR.Value;

        //var stopwatch = new Stopwatch();
        //stopwatch.Start();
        //await UploadJsonToS3Async(ads, "flvt", "/advertisements/ads-test.json");
        //stopwatch.Stop();

        //Console.WriteLine(stopwatch.ElapsedMilliseconds);

        //var stopwatch = new Stopwatch();
        //stopwatch.Start();
        //var ads = await RetrieveJsonFromS3Async("flvt", "/advertisements/ads-test.json");
        //stopwatch.Stop();

        //Console.WriteLine(stopwatch.ElapsedMilliseconds);
        //Console.WriteLine();

        //var ads = (await _repository.GetAllAsync()).Value.ToList();

        //var link = ads[0].Link;
        //var cmd = new FlagCommand(link);

        //await _repository.UpdateRangeAsync(ads);
        //Console.WriteLine((await _sender.Send(cmd)).IsSuccess);

        //TODO THIS WORKS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //var web = new HtmlWeb();

        //var a = await web.LoadFromWebAsync(
        //    "https://www.morizon.pl/oferta/wynajem-mieszkanie-warszawa-praga-polnoc-siedlecka-50m2-mzn2044602866");


        //var nodes = a.DocumentNode.SelectNodes("//script[@type='application/ld+json']").FirstOrDefault(node => node.InnerText.Contains("Offer"));

        //Console.WriteLine(nodes.InnerText);

        //var a = await _repository.GetAllAsync();

        //var all = a.Value.ToList();
        //var dist = all.DistinctBy(a => a.Dedupe).ToList();

        //var b = all.Except(dist).ToList();

        //Console.WriteLine(b.Count);

        //await _repository.RemoveRangeAsync(b.Select(a => a.Link));
    }

    public async Task UploadJsonToS3Async(IEnumerable<ProcessedAdvertisement> ads, string bucketName, string key)
    {
        var s3Client = new Amazon.S3.AmazonS3Client(new BasicAWSCredentials(XD.LOL2, XD.LOL3), RegionEndpoint.EUWest1);

        // Serialize the ads to JSON
        var json = JsonConvert.SerializeObject(ads, Formatting.Indented);

        // Create a MemoryStream from the JSON string
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Create the S3 PutObject request
        var putRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = stream,
            ContentType = "application/json"
        };

        // Upload to S3
        await s3Client.PutObjectAsync(putRequest);
    }
    public async Task<IEnumerable<ProcessedAdvertisement>> RetrieveJsonFromS3Async(string bucketName, string key)
    {
        var s3Client = new Amazon.S3.AmazonS3Client(new BasicAWSCredentials(XD.LOL2, XD.LOL3), RegionEndpoint.EUWest1);

        // Create a GetObject request
        var getRequest = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = key
        };

        // Fetch the file from S3
        using var response = await s3Client.GetObjectAsync(getRequest);
        await using var responseStream = response.ResponseStream;
        using var reader = new StreamReader(responseStream);
        // Read the content as a string
        var json = await reader.ReadToEndAsync();

        // Deserialize the JSON content to the target C# object type
        return JsonConvert.DeserializeObject<IEnumerable<ProcessedAdvertisement>>(json);
    }
}

public interface IService
{
    Task Run();
}