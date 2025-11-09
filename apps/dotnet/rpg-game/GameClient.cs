//using CheeseGrater;
//using Godot;
//using Grpc.Net.Client;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace rpggame;

//public class GameClient
//{
//	private readonly GameCommandService.GameCommandServiceClient _client;

//	public GameClient(string serverAddress)
//	{
//		var channel = GrpcChannel.ForAddress(serverAddress);
//		_client = new GameCommandService.GameCommandServiceClient(channel);
//	}


//	public async Task NotifyEquipChangeAsync(string playerId, string itemId)
//	{
//		var request = new EquipChangeRequest { PlayerId = playerId, ItemId = itemId };
//		await _client.NotifyEquipChangeAsync(request);
//		GD.Print($"Sent equip change: {playerId} -> {itemId}");
//	}
//}
