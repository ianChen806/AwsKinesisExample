using Amazon;
using AutoFixture;
using KinesisExample.Models;
using KinesisExample.Services;

// 建立 AWS 憑證服務
var credentialService = new AwsCredentialService();

try
{
    // 讀取 AWS 憑證
    var credentials = credentialService.GetCredentialsAsync();

    // AWS 設定
    var region = RegionEndpoint.APNortheast1; // 根據您的需求修改區域
    var streamName = "{stream-name}";

    // 建立 Kinesis 服務
    var kinesisService = new KinesisService(credentials, region, streamName);

    // 建立事件資料
    do
    {
        var userEvent = new Fixture()
            .Build<UserEvent>()
            .With(r => r.Action, "click")
            .Create();

        // 發送資料到 Kinesis
        await kinesisService.SendUserEventAsync(userEvent);
    } while (Console.ReadLine() != "end");
}
catch (FileNotFoundException ex)
{
    Console.WriteLine($"錯誤: {ex.Message}");
    Console.WriteLine("請確保您已經設定 AWS 憑證檔案。");
    Console.WriteLine("憑證檔案通常位於: ~/.aws/credentials");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"錯誤: {ex.Message}");
    Console.WriteLine("請確保您的 AWS 憑證檔案包含正確的存取金鑰設定。");
}
catch (Exception ex)
{
    Console.WriteLine($"程式執行時發生錯誤: {ex.Message}");
}
