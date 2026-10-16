using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects.Components;
public static class GeradorDeCPF
{
	public static string GerarCpf(bool comMascara = false)
	{
		var random = new Random();
		int soma = 0, resto;

		var cpf = new int[11];

		for (int i = 0; i < 9; i++)
		{
			cpf[i] = random.Next(0, 10);
			soma += cpf[i] * (10 - i);
		}

		resto = soma % 11;
		cpf[9] = (resto < 2) ? 0 : 11 - resto;

		soma = 0;
		for (int i = 0; i < 10; i++)
		{
			soma += cpf[i] * (11 - i);
		}

		resto = soma % 11;
		cpf[10] = (resto < 2) ? 0 : 11 - resto;

		string cpfStr = string.Join("", cpf);

		return comMascara
			? Convert.ToUInt64(cpfStr).ToString(@"000\.000\.000\-00")
			: cpfStr;
	}
}

