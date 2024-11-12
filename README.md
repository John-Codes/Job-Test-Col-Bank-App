# Bank terminal v0
Bank App is a comprehensive financial management application built with .NET, Angular, Docker, and Kubernetes. It offers users a seamless and secure platform to manage their banking needs, including account management, transaction tracking, and personalized financial insights. The app leverages modern web technologies to provide a robust and scalable solution, ensuring high availability and performance.
# Class Diagram
```mermaid
classDiagram
    %% Clases Principales del Banco
    class Banco {
        -List<Cuenta> cuentas
        -List<Cliente> clientes
        -List<Sucursal> sucursales
        -TableroControl tableroControl
        +crearCuentaAhorro(Cliente)
        +crearCuentaCorriente(Cliente)
        +generarReporteMensual(Date)
        +obtenerClientesAltoMovimiento(Month)
        +obtenerRetirosForaneos(double limite)
        +transferirDinero(Cuenta origen, Cuenta destino, double monto)
        +validarCuenta(String numeroCuenta)
    }

    class Sucursal {
        -String sucursalId
        -String nombre
        -String direccion
        -String ciudad
        -List<Empleado> empleados
        +obtenerTransaccionesSucursal(Date)
        +obtenerTotalDepositos()
        +obtenerTotalRetiros()
    }

    class Empleado {
        -String empleadoId
        -String nombre
        -String cargo
        -Sucursal sucursal
        +procesarTransaccion(Transaccion)
        +validarTransaccion(Transaccion)
    }

    class Cliente {
        -String clienteId
        -String nombre
        -String direccion
        -String ciudad
        -String telefono
        -String correo
        -TipoCliente tipo
        -List<Cuenta> cuentas
        -Date fechaRegistro
        +agregarCuenta(Cuenta)
        +obtenerNumeroTransacciones(Month)
        +actualizarPerfil(ClienteDTO)
        +validarIdentidad()
    }

    class Cuenta {
        <<abstract>>
        -String numeroCuenta
        -double saldo
        -Cliente propietario
        -Sucursal sucursalOrigen
        -EstadoCuenta estado
        -List<Transaccion> transacciones
        -Date fechaApertura
        +depositar(double monto, String ubicacion)
        +retirar(double monto, String ubicacion)
        +obtenerSaldo()
        +obtenerTransacciones(RangoFecha)
        +generarEstadoCuenta(Month)
        +validarTransaccion(Transaccion)
        +congelarCuenta()
        +descongelarCuenta()
    }

    class CuentaAhorro {
        -double tasaInteres
        -double saldoMinimo
        +calcularInteres()
        +aplicarInteres()
        +validarSaldoMinimo()
    }

    class CuentaCorriente {
        -double limiteDescubierto
        -double comisionMantenimiento
        +obtenerLimiteDescubierto()
        +aplicarComisionMantenimiento()
        +validarLimiteDescubierto()
    }

    class Transaccion {
        -String transaccionId
        -LocalDateTime fechaHora
        -TipoTransaccion tipo
        -EstadoTransaccion estado
        -double monto
        -String ubicacion
        -String descripcion
        -Empleado procesadoPor
        +obtenerDetalles()
        +validar()
        +revertir()
        +actualizarEstado(EstadoTransaccion)
    }

    %% Clases del Tablero de Control
    class TableroControl {
        -List<Reporte> reportes
        +generarReporte(TipoReporte, Date)
        +actualizarDatos()
        +exportarReporte(Reporte, Formato)
    }

    class Reporte {
        <<abstract>>
        -String reporteId
        -Date fechaGeneracion
        -EstadoReporte estado
        -Map<String, Object> filtros
        +generar()
        +obtenerResultados()
        +aplicarFiltros()
    }

    class ReporteVolumenTransacciones {
        -Month mesObjetivo
        -List<ResumenTransaccionesCliente> resumenes
        +generarReporteTransaccionesMensual()
        +obtenerClientesPorNumeroTransacciones()
        +exportarACSV()
    }

    class ReporteRetirosForaneos {
        -double montoLimite
        -List<ResumenRetiros> resumenes
        +generarReporteRetiros()
        +obtenerClientesSobreLimite()
        +calcularTotalRetiros()
    }

    class ResumenTransaccionesCliente {
        -Cliente cliente
        -int numeroTransacciones
        -Month mes
        -List<Transaccion> transacciones
        +obtenerDesglose()
        +calcularPromedioDiario()
    }

    class ResumenRetiros {
        -Cliente cliente
        -double montoTotal
        -List<Transaccion> retirosForaneos
        -String ciudadOrigen
        +obtenerUbicacionesRetiro()
        +calcularPromedioRetiros()
    }

    %% Enumeraciones
    class TipoTransaccion {
        <<enumeration>>
        DEPOSITO
        RETIRO
        TRANSFERENCIA_ENTRADA
        TRANSFERENCIA_SALIDA
        CREDITO_INTERES
        DEBITO_COMISION
    }

    class EstadoCuenta {
        <<enumeration>>
        ACTIVA
        CONGELADA
        CERRADA
        SUSPENDIDA
    }

    class EstadoTransaccion {
        <<enumeration>>
        PENDIENTE
        COMPLETADA
        FALLIDA
        REVERTIDA
    }

    class TipoCliente {
        <<enumeration>>
        INDIVIDUAL
        EMPRESARIAL
    }

    class TipoReporte {
        <<enumeration>>
        VOLUMEN_TRANSACCIONES_MENSUAL
        RETIROS_FORANEOS
        PERSONALIZADO
    }

    class Formato {
        <<enumeration>>
        PDF
        CSV
        EXCEL
        JSON
    }

    class EstadoReporte {
        <<enumeration>>
        GENERANDO
        COMPLETADO
        ERROR
        EXPIRADO
        CANCELADO
    }

    %% Relaciones
    Banco "1" -- "*" Sucursal
    Banco "1" -- "1" TableroControl
    Sucursal "1" -- "*" Empleado
    Banco "1" -- "*" Cliente
    Banco "1" -- "*" Cuenta
    Cliente "1" -- "*" Cuenta
    Cuenta "1" -- "*" Transaccion
    Sucursal "1" -- "*" Cuenta
    Empleado "1" -- "*" Transaccion
    Cuenta <|-- CuentaAhorro
    Cuenta <|-- CuentaCorriente
    TableroControl "1" -- "*" Reporte
    Reporte <|-- ReporteVolumenTransacciones
    Reporte <|-- ReporteRetirosForaneos
    ReporteVolumenTransacciones "1" -- "*" ResumenTransaccionesCliente
    ReporteRetirosForaneos "1" -- "*" ResumenRetiros
    ResumenTransaccionesCliente "*" -- "1" Cliente
    ResumenRetiros "*" -- "1" Cliente


``` 
