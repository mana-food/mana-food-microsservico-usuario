namespace ManaFood.Application.Utils;

public static class CpfUtils
{
        public static bool IsValidCpf(string cpf)
    {
        // Verifica se o CPF é nulo ou vazio
        if (string.IsNullOrWhiteSpace(cpf)) return false;
        cpf = new string(cpf.Where(char.IsDigit).ToArray());

        // Verifica se o CPF tem 11 dígitos
        if (cpf.Length != 11) return false;

        // Verifica se todos os dígitos são iguais
        if (cpf.All(c => c == cpf[0]))
            return false;

        // Calcula o primeiro dígito verificador
        int sum = 0;
        for (int i = 0; i < 9; i++)
            sum += (cpf[i] - '0') * (10 - i);

        int d1 = (sum * 10) % 11;
        if (d1 == 10)
            d1 = 0;

        if (d1 != (cpf[9] - '0'))
            return false;

        // Calcula o segundo dígito verificador
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += (cpf[i] - '0') * (11 - i);

        int d2 = (sum * 10) % 11;
        if (d2 == 10)
            d2 = 0;

        if (d2 != (cpf[10] - '0'))
            return false;

        return true;
    }
}