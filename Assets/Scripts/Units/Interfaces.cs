public interface ICustomButton {
    string CustomButtonName { get; }
    void CustomButton();
}


public interface IFuel {
    int fuel { get; set; }
    int maxFuel { get; set; }
    int fuelPerMove { get; set; }
}