using Ledgr.Domain.Entities;
using Ledgr.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Ledgr.Infrastructure.Persistence;

public static class LedgrDbSeeder
{
    public static async Task SeedAsync(LedgrDbContext db)
    {
        if (await db.Organizations.AnyAsync()) return;

        var hash = (string p) => BCrypt.Net.BCrypt.HashPassword(p);
        static DateTime Ago(int days, int h, int m = 0) =>
            DateTime.UtcNow.Date.AddDays(-days).AddHours(h).AddMinutes(m);

        // ── Org A — TechCorp ─────────────────────────────────────────────────
        var techcorp = new Organization { Name = "TechCorp" };
        db.Organizations.Add(techcorp);

        var tcAdmin   = new User { Name = "Alice Martins",  Email = "alice@techcorp.dev",   PasswordHash = hash("admin123"), Role = UserRole.Admin,   Org = techcorp };
        var tcManager = new User { Name = "Bruno Souza",    Email = "bruno@techcorp.dev",   PasswordHash = hash("mgr123"),   Role = UserRole.Manager, Org = techcorp };
        var tcAnalyst = new User { Name = "Carla Neves",    Email = "carla@techcorp.dev",   PasswordHash = hash("ana123"),   Role = UserRole.Analyst, Org = techcorp };
        db.Users.AddRange(tcAdmin, tcManager, tcAnalyst);

        var tcChecking = new Account { Name = "Conta Corrente",  Type = AccountType.Checking, Org = techcorp };
        var tcCredit   = new Account { Name = "Cartão Corporativo", Type = AccountType.Credit, Org = techcorp };
        var tcSavings  = new Account { Name = "Reserva",         Type = AccountType.Savings,  Org = techcorp };
        db.Accounts.AddRange(tcChecking, tcCredit, tcSavings);

        var tcSalary    = new Category { Name = "Salários",        Type = CategoryType.Expense, Color = "#EF4444", Org = techcorp };
        var tcInfra     = new Category { Name = "Infraestrutura",  Type = CategoryType.Expense, Color = "#F97316", Org = techcorp };
        var tcMarketing = new Category { Name = "Marketing",       Type = CategoryType.Expense, Color = "#EAB308", Org = techcorp };
        var tcRevenue   = new Category { Name = "Receita SaaS",    Type = CategoryType.Income,  Color = "#22C55E", Org = techcorp };
        var tcConsult   = new Category { Name = "Consultoria",     Type = CategoryType.Income,  Color = "#3B82F6", Org = techcorp };
        var tcOther     = new Category { Name = "Outros",          Type = CategoryType.Expense, Color = "#8B5CF6", Org = techcorp };
        db.Categories.AddRange(tcSalary, tcInfra, tcMarketing, tcRevenue, tcConsult, tcOther);

        var tcTx = new List<Transaction>
        {
            // Receitas — últimos 30 dias
            new() { Org=techcorp, Account=tcChecking, Category=tcRevenue,   Amount=28500, Type=TransactionType.Income,  Status=TransactionStatus.Confirmed, Date=Ago(28,9),    Description="Assinaturas março" },
            new() { Org=techcorp, Account=tcChecking, Category=tcRevenue,   Amount=31200, Type=TransactionType.Income,  Status=TransactionStatus.Confirmed, Date=Ago(14,9),    Description="Assinaturas — expansão Q2" },
            new() { Org=techcorp, Account=tcChecking, Category=tcConsult,   Amount=9500,  Type=TransactionType.Income,  Status=TransactionStatus.Confirmed, Date=Ago(21,10),   Description="Projeto Alpha — fase 1" },
            new() { Org=techcorp, Account=tcChecking, Category=tcConsult,   Amount=12000, Type=TransactionType.Income,  Status=TransactionStatus.Confirmed, Date=Ago(7,10),    Description="Projeto Beta — entrega" },
            new() { Org=techcorp, Account=tcChecking, Category=tcRevenue,   Amount=33800, Type=TransactionType.Income,  Status=TransactionStatus.Confirmed, Date=Ago(1,9),     Description="Assinaturas abril" },
            new() { Org=techcorp, Account=tcSavings,  Category=tcRevenue,   Amount=5000,  Type=TransactionType.Income,  Status=TransactionStatus.Pending,   Date=Ago(0,8),     Description="Juros reserva" },

            // Despesas — últimos 30 dias
            new() { Org=techcorp, Account=tcChecking, Category=tcSalary,    Amount=18000, Type=TransactionType.Expense, Status=TransactionStatus.Confirmed, Date=Ago(30,10),   Description="Folha março" },
            new() { Org=techcorp, Account=tcChecking, Category=tcInfra,     Amount=3200,  Type=TransactionType.Expense, Status=TransactionStatus.Confirmed, Date=Ago(27,11),   Description="AWS + serviços" },
            new() { Org=techcorp, Account=tcCredit,   Category=tcMarketing, Amount=4500,  Type=TransactionType.Expense, Status=TransactionStatus.Confirmed, Date=Ago(22,14),   Description="Google Ads" },
            new() { Org=techcorp, Account=tcChecking, Category=tcInfra,     Amount=1800,  Type=TransactionType.Expense, Status=TransactionStatus.Confirmed, Date=Ago(18,9),    Description="Licenças software" },
            new() { Org=techcorp, Account=tcCredit,   Category=tcOther,     Amount=750,   Type=TransactionType.Expense, Status=TransactionStatus.Confirmed, Date=Ago(15,16),   Description="Escritório — suprimentos" },
            new() { Org=techcorp, Account=tcChecking, Category=tcSalary,    Amount=18000, Type=TransactionType.Expense, Status=TransactionStatus.Confirmed, Date=Ago(1,10),    Description="Folha abril" },
            new() { Org=techcorp, Account=tcChecking, Category=tcMarketing, Amount=3800,  Type=TransactionType.Expense, Status=TransactionStatus.Confirmed, Date=Ago(4,13),    Description="LinkedIn Ads" },
            new() { Org=techcorp, Account=tcCredit,   Category=tcInfra,     Amount=2600,  Type=TransactionType.Expense, Status=TransactionStatus.Confirmed, Date=Ago(3,11),    Description="Datadog" },

            // Últimos 7 dias — visíveis no gráfico
            new() { Org=techcorp, Account=tcChecking, Category=tcConsult,   Amount=7500,  Type=TransactionType.Income,  Status=TransactionStatus.Confirmed, Date=Ago(6,9),     Description="Workshop remoto" },
            new() { Org=techcorp, Account=tcCredit,   Category=tcOther,     Amount=420,   Type=TransactionType.Expense, Status=TransactionStatus.Confirmed, Date=Ago(5,14),    Description="Evento tech" },
            new() { Org=techcorp, Account=tcChecking, Category=tcRevenue,   Amount=2200,  Type=TransactionType.Income,  Status=TransactionStatus.Confirmed, Date=Ago(4,10),    Description="Upgrade de plano — cliente X" },
            new() { Org=techcorp, Account=tcChecking, Category=tcInfra,     Amount=980,   Type=TransactionType.Expense, Status=TransactionStatus.Confirmed, Date=Ago(3,9),     Description="Domínios e SSL" },
            new() { Org=techcorp, Account=tcChecking, Category=tcConsult,   Amount=4800,  Type=TransactionType.Income,  Status=TransactionStatus.Confirmed, Date=Ago(2,11),    Description="Projeto Gamma — parcela 1" },
            new() { Org=techcorp, Account=tcCredit,   Category=tcMarketing, Amount=1200,  Type=TransactionType.Expense, Status=TransactionStatus.Pending,   Date=Ago(1,15),    Description="Meta Ads — pendente" },
            new() { Org=techcorp, Account=tcChecking, Category=tcRevenue,   Amount=1500,  Type=TransactionType.Income,  Status=TransactionStatus.Pending,   Date=Ago(0,8),     Description="Novo cliente — NF emitida" },
        };
        db.Transactions.AddRange(tcTx);

        // ── Org B — RetailCo ─────────────────────────────────────────────────
        var retailco = new Organization { Name = "RetailCo" };
        db.Organizations.Add(retailco);

        var rcAdmin   = new User { Name = "Diego Alves",    Email = "diego@retailco.dev",   PasswordHash = hash("admin123"), Role = UserRole.Admin,   Org = retailco };
        var rcManager = new User { Name = "Elena Costa",    Email = "elena@retailco.dev",   PasswordHash = hash("mgr123"),   Role = UserRole.Manager, Org = retailco };
        var rcAnalyst = new User { Name = "Fábio Lima",     Email = "fabio@retailco.dev",   PasswordHash = hash("ana123"),   Role = UserRole.Analyst, Org = retailco };
        db.Users.AddRange(rcAdmin, rcManager, rcAnalyst);

        var rcChecking = new Account { Name = "Caixa Geral",     Type = AccountType.Checking, Org = retailco };
        var rcCredit   = new Account { Name = "Cartão Empresarial", Type = AccountType.Credit, Org = retailco };
        var rcSavings  = new Account { Name = "Fundo de Reserva", Type = AccountType.Savings, Org = retailco };
        db.Accounts.AddRange(rcChecking, rcCredit, rcSavings);

        var rcSales     = new Category { Name = "Vendas",          Type = CategoryType.Income,  Color = "#22C55E", Org = retailco };
        var rcSuppliers = new Category { Name = "Fornecedores",    Type = CategoryType.Expense, Color = "#EF4444", Org = retailco };
        var rcLogistics = new Category { Name = "Logística",       Type = CategoryType.Expense, Color = "#F97316", Org = retailco };
        var rcPayroll   = new Category { Name = "Folha de Pag.",   Type = CategoryType.Expense, Color = "#EAB308", Org = retailco };
        var rcOnline    = new Category { Name = "Vendas Online",   Type = CategoryType.Income,  Color = "#3B82F6", Org = retailco };
        var rcOpsOther  = new Category { Name = "Operacional",     Type = CategoryType.Expense, Color = "#8B5CF6", Org = retailco };
        db.Categories.AddRange(rcSales, rcSuppliers, rcLogistics, rcPayroll, rcOnline, rcOpsOther);

        var rcTx = new List<Transaction>
        {
            new() { Org=retailco, Account=rcChecking, Category=rcSales,     Amount=42000, Type=TransactionType.Income,  Status=TransactionStatus.Confirmed, Date=Ago(29,8),  Description="Vendas loja física — março" },
            new() { Org=retailco, Account=rcChecking, Category=rcOnline,    Amount=18500, Type=TransactionType.Income,  Status=TransactionStatus.Confirmed, Date=Ago(26,10), Description="E-commerce março" },
            new() { Org=retailco, Account=rcChecking, Category=rcSuppliers, Amount=22000, Type=TransactionType.Expense, Status=TransactionStatus.Confirmed, Date=Ago(25,9),  Description="Reposição estoque" },
            new() { Org=retailco, Account=rcCredit,   Category=rcLogistics, Amount=5400,  Type=TransactionType.Expense, Status=TransactionStatus.Confirmed, Date=Ago(23,11), Description="Frete e distribuição" },
            new() { Org=retailco, Account=rcChecking, Category=rcPayroll,   Amount=31000, Type=TransactionType.Expense, Status=TransactionStatus.Confirmed, Date=Ago(20,10), Description="Folha março" },
            new() { Org=retailco, Account=rcChecking, Category=rcSales,     Amount=38500, Type=TransactionType.Income,  Status=TransactionStatus.Confirmed, Date=Ago(14,8),  Description="Vendas loja — 1ª quinzena abril" },
            new() { Org=retailco, Account=rcChecking, Category=rcOnline,    Amount=21000, Type=TransactionType.Income,  Status=TransactionStatus.Confirmed, Date=Ago(10,10), Description="E-commerce — promoção páscoa" },
            new() { Org=retailco, Account=rcCredit,   Category=rcOpsOther,  Amount=1800,  Type=TransactionType.Expense, Status=TransactionStatus.Confirmed, Date=Ago(9,14),  Description="Manutenção equipamentos" },
            new() { Org=retailco, Account=rcChecking, Category=rcSuppliers, Amount=19500, Type=TransactionType.Expense, Status=TransactionStatus.Confirmed, Date=Ago(7,9),   Description="Fornecedor B — pedido mensal" },
            new() { Org=retailco, Account=rcChecking, Category=rcLogistics, Amount=4200,  Type=TransactionType.Expense, Status=TransactionStatus.Confirmed, Date=Ago(5,11),  Description="Transportadora" },
            new() { Org=retailco, Account=rcChecking, Category=rcSales,     Amount=15000, Type=TransactionType.Income,  Status=TransactionStatus.Confirmed, Date=Ago(4,9),   Description="Vendas loja — 2ª quinzena abril" },
            new() { Org=retailco, Account=rcChecking, Category=rcOnline,    Amount=9800,  Type=TransactionType.Income,  Status=TransactionStatus.Confirmed, Date=Ago(3,10),  Description="E-commerce dia 27" },
            new() { Org=retailco, Account=rcChecking, Category=rcPayroll,   Amount=31000, Type=TransactionType.Expense, Status=TransactionStatus.Confirmed, Date=Ago(1,10),  Description="Folha abril" },
            new() { Org=retailco, Account=rcCredit,   Category=rcOpsOther,  Amount=650,   Type=TransactionType.Expense, Status=TransactionStatus.Pending,   Date=Ago(1,15),  Description="Limpeza e conservação" },
            new() { Org=retailco, Account=rcChecking, Category=rcSales,     Amount=4200,  Type=TransactionType.Income,  Status=TransactionStatus.Pending,   Date=Ago(0,8),   Description="Vendas hoje — parcial" },
        };
        db.Transactions.AddRange(rcTx);

        await db.SaveChangesAsync();
    }
}
