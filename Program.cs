using System;
using System.Collections.Generic;
using System.Linq;

// Enums
public enum TipoCliente { INDIVIDUAL, EMPRESARIAL }
public enum EstadoCuenta { ACTIVA, CONGELADA, CERRADA, SUSPENDIDA }
public enum TipoTransaccion { DEPOSITO, RETIRO, TRANSFERENCIA_ENTRADA, TRANSFERENCIA_SALIDA }
public enum TipoReporte { VOLUMEN_TRANSACCIONES_MENSUAL, RETIROS_FORANEOS, PERSONALIZADO }

// Base Classes
public abstract class Cuenta
{
    public string NumeroCuenta { get; set; }
    public double Saldo { get; protected set; }
    public Cliente Propietario { get; set; }
    public EstadoCuenta Estado { get; set; }
    public List<Transaccion> Transacciones { get; set; } = new List<Transaccion>();

    public virtual bool Depositar(double monto, string ubicacion)
    {
        if (monto <= 0 || Estado != EstadoCuenta.ACTIVA) return false;

        Saldo += monto;
        Transacciones.Add(new Transaccion
        {
            FechaHora = DateTime.Now,
            Tipo = TipoTransaccion.DEPOSITO,
            Monto = monto,
            Ubicacion = ubicacion
        });
        return true;
    }

    public virtual bool Retirar(double monto, string ubicacion)
    {
        if (monto <= 0 || Estado != EstadoCuenta.ACTIVA || Saldo < monto) return false;

        Saldo -= monto;
        Transacciones.Add(new Transaccion
        {
            FechaHora = DateTime.Now,
            Tipo = TipoTransaccion.RETIRO,
            Monto = monto,
            Ubicacion = ubicacion
        });
        return true;
    }
}

public class CuentaAhorro : Cuenta
{
    public double TasaInteres { get; set; } = 0.02; // 2% interest rate
    public double SaldoMinimo { get; set; } = 100;

    public void AplicarInteres()
    {
        if (Estado == EstadoCuenta.ACTIVA && Saldo >= SaldoMinimo)
        {
            double interes = Saldo * TasaInteres;
            Saldo += interes;
        }
    }
}

public class Cliente
{
    public string ClienteId { get; set; }
    public string Nombre { get; set; }
    public string Correo { get; set; }
    public TipoCliente Tipo { get; set; }
    public List<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
}

public class Transaccion
{
    public DateTime FechaHora { get; set; }
    public TipoTransaccion Tipo { get; set; }
    public double Monto { get; set; }
    public string Ubicacion { get; set; }
}

public class Banco
{
    private List<Cliente> clientes = new List<Cliente>();
    private List<Cuenta> cuentas = new List<Cuenta>();

    public Cliente RegistrarCliente(string nombre, string correo, TipoCliente tipo)
    {
        var cliente = new Cliente
        {
            ClienteId = Guid.NewGuid().ToString().Substring(0, 8), // Shorter ID for easier testing
            Nombre = nombre,
            Correo = correo,
            Tipo = tipo
        };
        clientes.Add(cliente);
        return cliente;
    }

    public CuentaAhorro CrearCuentaAhorro(Cliente cliente)
    {
        var cuenta = new CuentaAhorro
        {
            NumeroCuenta = Guid.NewGuid().ToString().Substring(0, 8),
            Propietario = cliente,
            Estado = EstadoCuenta.ACTIVA
        };
        cliente.Cuentas.Add(cuenta);
        cuentas.Add(cuenta);
        return cuenta;
    }

    public List<Transaccion> ObtenerTransacciones(string clienteId)
    {
        return cuentas
            .Where(c => c.Propietario.ClienteId == clienteId)
            .SelectMany(c => c.Transacciones)
            .OrderByDescending(t => t.FechaHora)
            .ToList();
    }

    public Cliente? ObtenerCliente(string clienteId)
    {
        return clientes.FirstOrDefault(c => c.ClienteId == clienteId);
    }

    public List<Cliente> ObtenerTodosClientes()
    {
        return clientes;
    }
}

public class Program
{
    private static Banco banco = new Banco();
    private static Cliente? clienteActual = null;
    private static string testClientId = ""; // Will store our test client ID

    public static void Main()
    {
        // Create test client
        CrearClientePrueba();

        while (true)
        {
            if (clienteActual == null)
            {
                MostrarMenuInicial();
            }
            else
            {
                MostrarMenuCliente();
            }
        }
    }

    private static void CrearClientePrueba()
    {
        var clientePrueba = banco.RegistrarCliente("Usuario Prueba", "test@test.com", TipoCliente.INDIVIDUAL);
        var cuentaPrueba = banco.CrearCuentaAhorro(clientePrueba);
        testClientId = clientePrueba.ClienteId;

        // Add some test transactions
        cuentaPrueba.Depositar(1000, "Sucursal Central");
        cuentaPrueba.Retirar(200, "ATM");
        cuentaPrueba.Depositar(500, "Depósito Online");

        // Display test credentials
        Console.WriteLine("=== Credenciales de Prueba ===");
        Console.WriteLine($"ID de Cliente de Prueba: {testClientId}");
        Console.WriteLine("Contraseña de Admin: admin123");
        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    private static void MostrarMenuInicial()
    {
        Console.Clear();
        Console.WriteLine("=== Sistema Bancario ===");
        Console.WriteLine($"Cliente de Prueba ID: {testClientId}");
        Console.WriteLine("1. Iniciar sesión como cliente");
        Console.WriteLine("2. Iniciar sesión como administrador");
        Console.WriteLine("3. Registrar nuevo cliente");
        Console.WriteLine("4. Salir");

        switch (Console.ReadLine())
        {
            case "1":
                IniciarSesionCliente();
                break;
            case "2":
                IniciarSesionAdmin();
                break;
            case "3":
                RegistrarCliente();
                break;
            case "4":
                Environment.Exit(0);
                break;
        }
    }

    private static void MostrarMenuCliente()
    {
        Console.Clear();
        Console.WriteLine($"=== Bienvenido {clienteActual.Nombre} ===");
        Console.WriteLine($"ID de Cliente: {clienteActual.ClienteId}");
        Console.WriteLine("1. Ver saldo");
        Console.WriteLine("2. Depositar");
        Console.WriteLine("3. Retirar");
        Console.WriteLine("4. Ver transacciones");
        Console.WriteLine("5. Cerrar sesión");

        switch (Console.ReadLine())
        {
            case "1":
                MostrarSaldo();
                break;
            case "2":
                RealizarDeposito();
                break;
            case "3":
                RealizarRetiro();
                break;
            case "4":
                MostrarTransacciones();
                break;
            case "5":
                clienteActual = null;
                break;
        }
    }

    private static void IniciarSesionCliente()
    {
        Console.Write("Ingrese ID de cliente: ");
        string clienteId = Console.ReadLine();
        clienteActual = banco.ObtenerCliente(clienteId);

        if (clienteActual == null)
        {
            Console.WriteLine("Cliente no encontrado.");
            Console.ReadKey();
        }
    }

    private static void IniciarSesionAdmin()
    {
        Console.Write("Ingrese contraseña de administrador: ");
        if (Console.ReadLine() == "admin123")
        {
            MostrarMenuAdmin();
        }
        else
        {
            Console.WriteLine("Contraseña incorrecta.");
            Console.ReadKey();
        }
    }

    private static void MostrarMenuAdmin()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Panel de Administrador ===");
            Console.WriteLine("1. Ver todos los clientes");
            Console.WriteLine("2. Ver transacciones por cliente");
            Console.WriteLine("3. Volver al menú principal");

            switch (Console.ReadLine())
            {
                case "1":
                    MostrarTodosClientes();
                    break;
                case "2":
                    MostrarTransaccionesAdmin();
                    break;
                case "3":
                    return;
            }
        }
    }

    private static void RegistrarCliente()
    {
        Console.Write("Nombre: ");
        string nombre = Console.ReadLine();
        Console.Write("Correo: ");
        string correo = Console.ReadLine();

        var cliente = banco.RegistrarCliente(nombre, correo, TipoCliente.INDIVIDUAL);
        var cuenta = banco.CrearCuentaAhorro(cliente);

        Console.WriteLine($"Cliente registrado con éxito. ID: {cliente.ClienteId}");
        Console.WriteLine($"Cuenta de ahorro creada. Número: {cuenta.NumeroCuenta}");
        Console.ReadKey();
    }

    private static void MostrarSaldo()
    {
        if (clienteActual?.Cuentas.FirstOrDefault() is Cuenta cuenta)
        {
            Console.WriteLine($"Saldo actual: ${cuenta.Saldo}");
            Console.ReadKey();
        }
    }

    private static void RealizarDeposito()
    {
        if (clienteActual?.Cuentas.FirstOrDefault() is Cuenta cuenta)
        {
            Console.Write("Monto a depositar: $");
            if (double.TryParse(Console.ReadLine(), out double monto))
            {
                if (cuenta.Depositar(monto, "ATM"))
                {
                    Console.WriteLine("Depósito realizado con éxito.");
                }
                else
                {
                    Console.WriteLine("No se pudo realizar el depósito.");
                }
            }
            Console.ReadKey();
        }
    }

    private static void RealizarRetiro()
    {
        if (clienteActual?.Cuentas.FirstOrDefault() is Cuenta cuenta)
        {
            Console.Write("Monto a retirar: $");
            if (double.TryParse(Console.ReadLine(), out double monto))
            {
                if (cuenta.Retirar(monto, "ATM"))
                {
                    Console.WriteLine("Retiro realizado con éxito.");
                }
                else
                {
                    Console.WriteLine("No se pudo realizar el retiro.");
                }
            }
            Console.ReadKey();
        }
    }

    private static void MostrarTransacciones()
    {
        var transacciones = banco.ObtenerTransacciones(clienteActual.ClienteId);
        foreach (var t in transacciones)
        {
            Console.WriteLine($"{t.FechaHora}: {t.Tipo} - ${t.Monto} - {t.Ubicacion}");
        }
        Console.ReadKey();
    }

    private static void MostrarTodosClientes()
    {
        foreach (var cliente in banco.ObtenerTodosClientes())
        {
            Console.WriteLine($"ID: {cliente.ClienteId}, Nombre: {cliente.Nombre}, Email: {cliente.Correo}");
            if (cliente.Cuentas.FirstOrDefault() is Cuenta cuenta)
            {
                Console.WriteLine($"Saldo: ${cuenta.Saldo}");
            }
            Console.WriteLine();
        }
        Console.ReadKey();
    }

    private static void MostrarTransaccionesAdmin()
    {
        Console.Write("Ingrese ID del cliente: ");
        string clienteId = Console.ReadLine();
        var transacciones = banco.ObtenerTransacciones(clienteId);

        foreach (var t in transacciones)
        {
            Console.WriteLine($"{t.FechaHora}: {t.Tipo} - ${t.Monto} - {t.Ubicacion}");
        }
        Console.ReadKey();
    }
}