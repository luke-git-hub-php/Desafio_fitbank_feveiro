using System;
using pokemon.Entities;

namespace pokemon
{
    public class Program : Links
    {
        public static void Main(string[]args)
        {
            Console.WriteLine("Quantas Páginas deseja fazer a varredura no site?");
            var num = Console.ReadLine(); 
            int options = 0;
            while (options < 1) {
                Console.WriteLine("Dejesa salvar os dados das cartas em arquivo único? 1-SIM 2-NAO");
                options = Convert.ToInt32(Console.ReadLine());
                if (options > 2)
                {
                    Console.WriteLine("A Opção é inválida!");
                    options = 0;
                } 
            }
            Links urlCard = Cards(Convert.ToInt32(num));
            CardsData(urlCard, Convert.ToInt32(num), options);
        }
    }
}
