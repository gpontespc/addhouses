using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tibia.Protobuf.Staticdata;

namespace AddHouses
{
	internal class Program
	{
		public static StaticData GlobalStaticData { get; set; }
		public static List<House> Houses => GlobalStaticData.House.ToList();

		static void Main(string[] args)
		{
			// Carrega os dados
			LoadData();

			// Menu principal
			bool running = true;
			while (running)
			{
				Console.Clear();
				Console.WriteLine("=== MENU DE GERENCIAMENTO DE CASAS ===");
				Console.WriteLine("1. Listar todas as casas");
				Console.WriteLine("2. Procurar casa por ID");
				Console.WriteLine("3. Procurar casas por nome");
				Console.WriteLine("4. Procurar casas por cidade");
				Console.WriteLine("5. Mostrar casas ordenadas por preço");
				Console.WriteLine("6. Mostrar casas ordenadas por tamanho");
				Console.WriteLine("7. Adicionar nova casa");
				Console.WriteLine("8. Remover casa");
				Console.WriteLine("9. Salvar");
				Console.WriteLine("10. Sair");
				Console.Write("\nEscolha uma opção: ");

				var option = Console.ReadLine();
				Console.WriteLine();

				switch (option)
				{
					case "1":
						ListAllHouses();
						break;
					case "2":
						SearchHouseById();
						break;
					case "3":
						SearchHouseByName();
						break;
					case "4":
						SearchHouseByCity();
						break;
					case "5":
						ShowHousesOrderedByPrice();
						break;
					case "6":
						ShowHousesOrderedBySize();
						break;
					case "7":
						AddNewHouse();
						break;
					case "8":
						RemoveHouse();
						break;
					case "9":
						SaveData();
						break;
					case "10":
						running = false;
						break;
					default:
						Console.WriteLine("Opção inválida. Tente novamente.");
						break;
				}

				if (running && !option.Equals("9"))
				{
					Console.WriteLine("\nPressione qualquer tecla para continuar...");
					Console.ReadKey();
				}
			}

			// Salva os dados ao sair
			SaveData();
		}

		static void LoadData()
		{
			try
			{
				using (FileStream fileStream = new FileStream("C:/Users/gewbi/Downloads/staticdata-96f4677aaff232cba831ae79e52f9752e060281391fbd77c5e56ad498ccb7c4b.dat", FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
				{
					GlobalStaticData = StaticData.Parser.ParseFrom(fileStream);
				}
				Console.WriteLine("Dados carregados com sucesso!");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Erro ao carregar dados: {ex.Message}");
				Environment.Exit(1);
			}
		}

		static void SaveData()
		{
			try
			{
				using (FileStream fileStream = new FileStream("C:/Users/gewbi/Downloads/staticdata-96f4677aaff232cba831ae79e52f9752e060281391fbd77c5e56ad498ccb7c4b.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
				{
					GlobalStaticData.WriteTo(fileStream);
				}
				Console.WriteLine("Dados salvos com sucesso!");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Erro ao salvar dados: {ex.Message}");
			}
		}

		static void ListAllHouses()
		{
			Console.WriteLine("=== LISTA DE TODAS AS CASAS ===");
			foreach (var house in Houses)
			{
				PrintHouseDetails(house);
			}
			Console.WriteLine($"\nTotal de casas: {Houses.Count}");
		}

		static void SearchHouseById()
		{
			Console.Write("Digite o ID da casa: ");
			if (uint.TryParse(Console.ReadLine(), out uint houseId))
			{
				var house = Houses.FirstOrDefault(h => h.HouseId == houseId);
				if (house != null)
				{
					PrintHouseDetails(house);
				}
				else
				{
					Console.WriteLine($"Casa com ID {houseId} não encontrada.");
				}
			}
			else
			{
				Console.WriteLine("ID inválido. Digite um número válido.");
			}
		}

		static void SearchHouseByName()
		{
			Console.Write("Digite o nome (ou parte do nome) da casa: ");
			var searchTerm = Console.ReadLine().Trim();
			var results = Houses.Where(h => h.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

			if (results.Any())
			{
				Console.WriteLine($"=== RESULTADOS PARA '{searchTerm}' ===");
				foreach (var house in results)
				{
					PrintHouseDetails(house);
				}
				Console.WriteLine($"\nTotal encontrado: {results.Count}");
			}
			else
			{
				Console.WriteLine($"Nenhuma casa encontrada com o termo '{searchTerm}'.");
			}
		}

		static void SearchHouseByCity()
		{
			Console.Write("Digite o nome da cidade: ");
			var city = Console.ReadLine().Trim();
			var results = Houses.Where(h => h.City.Equals(city, StringComparison.OrdinalIgnoreCase)).ToList();

			if (results.Any())
			{
				Console.WriteLine($"=== CASAS EM {city.ToUpper()} ===");
				foreach (var house in results)
				{
					PrintHouseDetails(house);
				}
				Console.WriteLine($"\nTotal em {city}: {results.Count}");
			}
			else
			{
				Console.WriteLine($"Nenhuma casa encontrada na cidade '{city}'.");
			}
		}

		static void ShowHousesOrderedByPrice()
		{
			var ordered = Houses.OrderBy(h => h.Price).ToList();
			Console.WriteLine("=== CASAS ORDENADAS POR PREÇO (CRESCENTE) ===");
			foreach (var house in ordered)
			{
				Console.WriteLine($"ID: {house.HouseId} | Nome: {house.Name} | Preço: {house.Price} | Tamanho: {house.SizeSqm} m² | Cidade: {house.City}");
			}
		}

		static void ShowHousesOrderedBySize()
		{
			var ordered = Houses.OrderByDescending(h => h.SizeSqm).ToList();
			Console.WriteLine("=== CASAS ORDENADAS POR TAMANHO (DECRESCENTE) ===");
			foreach (var house in ordered)
			{
				Console.WriteLine($"ID: {house.HouseId} | Nome: {house.Name} | Tamanho: {house.SizeSqm} m² | Preço: {house.Price} | Cidade: {house.City}");
			}
		}

		static void AddNewHouse()
		{
			Console.WriteLine("=== ADICIONAR NOVA CASA ===");

			try
			{
				var newHouse = new House();

				Console.Write("Nome da casa: ");
				newHouse.Name = Console.ReadLine();

				Console.Write("ID da casa: ");
				newHouse.HouseId = uint.Parse(Console.ReadLine());

				Console.Write("Preço: ");
				newHouse.Price = uint.Parse(Console.ReadLine());

				Console.Write("Número de camas: ");
				newHouse.Beds = uint.Parse(Console.ReadLine());

				Console.Write("Tamanho (m²): ");
				newHouse.SizeSqm = uint.Parse(Console.ReadLine());

				Console.Write("Cidade: ");
				newHouse.City = Console.ReadLine();

				Console.Write("É guildhall? (S/N): ");
				newHouse.Guildhall = Console.ReadLine().Trim().Equals("S", StringComparison.OrdinalIgnoreCase);

				Console.Write("É loja? (S/N): ");
				newHouse.Shop = Console.ReadLine().Trim().Equals("S", StringComparison.OrdinalIgnoreCase);

				Console.WriteLine("\nPosição da casa:");
				Console.Write("PosX: ");
				var posX = uint.Parse(Console.ReadLine()); // Alterado para uint.Parse
				Console.Write("PosY: ");
				var posY = uint.Parse(Console.ReadLine()); // Alterado para uint.Parse
				Console.Write("PosZ: ");
				var posZ = uint.Parse(Console.ReadLine()); // Alterado para uint.Parse

				newHouse.HousePosition = new HousePosition
				{
					PosX = posX,
					PosY = posY,
					PosZ = posZ
				};

				GlobalStaticData.House.Add(newHouse);
				Console.WriteLine("\nCasa adicionada com sucesso!");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Erro ao adicionar casa: {ex.Message}");
			}
		}

		static void RemoveHouse()
		{
			Console.Write("Digite o ID da casa que deseja remover: ");
			if (uint.TryParse(Console.ReadLine(), out uint houseId))
			{
				var house = Houses.FirstOrDefault(h => h.HouseId == houseId);
				if (house != null)
				{
					GlobalStaticData.House.Remove(house);
					Console.WriteLine($"Casa {house.Name} (ID: {house.HouseId}) removida com sucesso.");
				}
				else
				{
					Console.WriteLine($"Casa com ID {houseId} não encontrada.");
				}
			}
			else
			{
				Console.WriteLine("ID inválido. Digite um número válido.");
			}
		}

		static void PrintHouseDetails(House house)
		{
			Console.WriteLine($"\nID: {house.HouseId}");
			Console.WriteLine($"Nome: {house.Name}");
			Console.WriteLine($"Preço: {house.Price}");
			Console.WriteLine($"Camas: {house.Beds}");
			Console.WriteLine($"Tamanho: {house.SizeSqm} m²");
			Console.WriteLine($"Posição: X={house.HousePosition.PosX}, Y={house.HousePosition.PosY}, Z={house.HousePosition.PosZ}");
			Console.WriteLine($"Cidade: {house.City}");
			Console.WriteLine($"Guildhall: {(house.Guildhall ? "Sim" : "Não")}");
			Console.WriteLine($"Loja: {(house.Shop ? "Sim" : "Não")}");
			Console.WriteLine(new string('-', 40));
		}
	}
}