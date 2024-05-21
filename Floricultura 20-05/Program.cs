using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Windows;
using System.Media;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using System.Numerics;

namespace FloriculturaConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Inicialização do banco de dados
            var client = new MongoClient("mongodb+srv://pablo23:TAoXAMkE464500Sdy@cluster0.xybwrdc.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0");
            var database = client.GetDatabase("pablo23");
            var collection = database.GetCollection<Planta>("floricultura");

            // Reprodução da música


            // Menu da Pokedex
            bool sair = false;
            while (!sair)
            {
                Console.WriteLine("========== Pokedex ==========");
                Console.WriteLine("1. Adicionar Planta");
                Console.WriteLine("2. Buscar Planta por nome");
                Console.WriteLine("3. Listar Plantas por família");
                Console.WriteLine("4. Listar todas as Plantas");
                Console.WriteLine("5 Atualizar Planta");
                Console.WriteLine("6. Deletar Pokemon");
                Console.WriteLine("7. Sair");
                Console.WriteLine("=============================");
                Console.Write("Escolha uma opção: ");
                var opcao = Console.ReadLine();
                Console.WriteLine();

                switch (opcao)
                {
                    case "1":
                        await AdicionarPlanta(collection);
                        break;
                    case "2":
                        await BuscarPlantaPorNome(collection);
                        break;
                    case "3":
                        await BuscarPlantasPorFamilia(collection);
                        break;
                    case "4":
                        await ListarTodasPlantas(collection);
                        break;
                    case "5":
                        await AtualizarPlanta(collection);
                        break;
                    case "6":
                        await DeletarPlanta(collection);
                        break;
                    case "7":
                        sair = true;
                        break;
                    default:
                        Console.WriteLine("Opção inválida.");
                        break;
                }

                Console.WriteLine();
            }
        }

        static async Task AdicionarPlanta(IMongoCollection<Planta> collection)
        {
            Console.Write("Nome Científico:");
            var nomeCientifico = Console.ReadLine();
            Console.Write("Nome Comum:");
            var nomeComum = Console.ReadLine();
            Console.Write("Familia:");
            var familia = Console.ReadLine();
            Console.Write("Origem:");
            var origem = Console.ReadLine();
            Console.Write("Descrição:");
            var descricao = Console.ReadLine();
            Console.Write("Url da Imagem:");
            var imagem = Console.ReadLine();

            var planta = new Planta
            {
                NomeCientifico = nomeCientifico,
                NomeComum = nomeComum,
                Familia = familia,
                Origem = origem,
                Descricao = descricao,
                Imagem = imagem
            };

            await collection.InsertOneAsync(planta);
            Console.WriteLine("Planta adicionada com sucesso:");
        }

        static async Task BuscarPlantaPorNome(IMongoCollection<Planta> collection)
        {
            Console.Write("Digite o nome cientifico ou comun da planta:");
            var nome = Console.ReadLine();
            var filter = Builders<Planta>.Filter.Or(Builders<Planta>.Filter.Eq(p => p.NomeCientifico, nome), Builders<Planta>.Filter.Eq(p => p.NomeComum, nome));
            var planta = await collection.Find(filter).FirstOrDefaultAsync();
            if (planta != null)
            {
                Console.WriteLine($"Planta econtrada");
                Console.WriteLine($"Nome Científico: {planta.NomeCientifico}");
                Console.WriteLine($"Nome Comum: {planta.NomeComum}");
                Console.WriteLine($"Família: {planta.Familia}");
                Console.WriteLine($"Origem: {planta.Origem}");
                Console.WriteLine($"Descrição: {planta.Descricao}");
            }
            else
            {
                Console.WriteLine("Planta não encontrado.");
            }
        }
        
        static async Task BuscarPlantasPorFamilia(IMongoCollection<Planta> collection)
        {
            Console.Write("Digite o nome da família:");
            var familia = Console.ReadLine();
            var filter = Builders<Planta>.Filter.Eq(p => p.Familia, familia);
            var plantas = await collection.Find(filter).ToListAsync();
            if (plantas != null)
            {
                foreach (var planta in plantas)
                {
                    Console.WriteLine($"Nome Científico: {planta.NomeCientifico}");
                    Console.WriteLine($"Nome Comum: {planta.NomeComum}");
                    Console.WriteLine($"Família: {planta.Familia}");
                    Console.WriteLine($"Origem: {planta.Origem}");
                    Console.WriteLine($"Descrição: {planta.Descricao}");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Família não encontrado.");
            }
            
        }

        static async Task ListarTodasPlantas(IMongoCollection<Planta> collection)
        {
            var plantas = await collection.Find(_ => true).ToListAsync();
            foreach (var planta in plantas)
            {
                Console.WriteLine($"Nome Científico: {planta.NomeCientifico}");
                Console.WriteLine($"Nome Comum: {planta.NomeComum}");
                Console.WriteLine($"Família: {planta.Familia}");
                Console.WriteLine($"Origem: {planta.Origem}");
                Console.WriteLine($"Descrição: {planta.Descricao}");
                Console.WriteLine();
            }
        }

        static async Task AtualizarPlanta(IMongoCollection<Planta> collection)
        {
            Console.Write("Digite o nome cientifico ou comum da planta que deseja atualizar:");
            var nome = Console.ReadLine();
            var filter = Builders<Planta>.Filter.Or(Builders<Planta>.Filter.Eq(p => p.NomeComum, nome), Builders<Planta>.Filter.Eq(p => p.NomeCientifico, nome));
            var planta = await collection.Find(filter).FirstOrDefaultAsync();
            if (planta != null)
            {
                Console.Write("Digite a nova família da planta:");
                var novaFamilia = Console.ReadLine();
                Console.Write("Digite a nova origem da planta:");
                var novaOrigem = Console.ReadLine();
                Console.Write("Digite a nova descrição:");
                var novaDescricao = Console.ReadLine();
                

                var update = Builders<Planta>.Update.Set(p => p.Familia, novaFamilia).Set(p => p.Origem, novaOrigem).Set(p => p.Descricao, novaDescricao);
                await collection.UpdateOneAsync(filter, update);
                Console.WriteLine("Planta atualizada com sucesso:");
            }
            else
            {
                Console.WriteLine("Planta não encontrado:");
            }
        }

        static async Task DeletarPlanta(IMongoCollection<Planta> collection)
        {
            Console.Write("Digite o nome cientifico ou comun da planta que deseja deletar: ");
            var nome = Console.ReadLine();
            var filter = Builders<Planta>.Filter.Or(Builders<Planta>.Filter.Eq(p => p.NomeComum, nome), Builders< Planta >.Filter.Eq(p => p.NomeCientifico, nome));
            var result = await collection.DeleteOneAsync(filter);

            if (result.DeletedCount > 0)
            {
                Console.WriteLine("Planta deletada com sucesso.");
            }
            else
            {
                Console.WriteLine("Planta não encontrada.");
            }
        }
    }
}
public class Planta
{
    public ObjectId Id { get; set; }
    public string NomeCientifico { get; set; }
    public string NomeComum { get; set; }
    public string Familia { get; set; }
    public string Origem { get; set; }
    public string Descricao { get; set; }
    public string Imagem { get; set; }
}


