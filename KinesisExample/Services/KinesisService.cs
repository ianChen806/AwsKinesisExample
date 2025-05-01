using System.Text;
using System.Text.Json;
using Amazon;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Amazon.Runtime;
using KinesisExample.Models;

namespace KinesisExample.Services;

public class KinesisService
{
    private readonly RegionEndpoint _region;
    private readonly string _streamName;
    private readonly AWSCredentials _credentials;

    public KinesisService(AWSCredentials credentials, RegionEndpoint region, string streamName)
    {
        _region = region;
        _streamName = streamName;
        _credentials = credentials;
    }

    public async Task SendUserEventAsync(UserEvent userEvent)
    {
        using var kinesisClient = new AmazonKinesisClient(_credentials, new AmazonKinesisConfig()
        {
            RegionEndpoint = _region
        });

        // 將事件資料轉換為 JSON
        var jsonData = JsonSerializer.Serialize(userEvent);
        var data = Encoding.UTF8.GetBytes(jsonData);

        // 建立 PutRecord 請求
        var putRecordRequest = new PutRecordRequest
        {
            StreamName = _streamName,
            Data = new MemoryStream(data),
            PartitionKey = userEvent.UserId // 使用 UserId 作為分區鍵
        };

        try
        {
            // 發送資料到 Kinesis
            var response = await kinesisClient.PutRecordAsync(putRecordRequest);
            Console.WriteLine($"資料已成功發送到 Kinesis。序列號: {response.SequenceNumber}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"發送資料時發生錯誤: {ex.Message}");
            throw;
        }
    }
}
