using System.Collections.Generic;

public interface ICustomButton {
    string CustomButtonName { get; }
    void CustomButton();
}


public interface IFuel {
    int fuel { get; set; }
    int maxFuel { get; set; }
    int fuelPerMove { get; set; }
}

public interface ITransport {
    UnitType unitOnTransportType { get; }
    List<Unit> unitsOnTransport { get; set; }
    int maxNumberOfUnits { get; }
    bool isTransportFull { get; }
    bool transportUIVisible { get; }
}