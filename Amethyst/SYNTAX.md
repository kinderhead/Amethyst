# Amethyst Syntax

Example program:
```cs
namespace test;

class Test {
    private short prop;

    public Test(short arg) {
        prop = arg;
    }

    public short Method() {
        return prop;
    }
}

int globalVariable = 324;

// #<tag>. #load and #tick resolve to minecraft:load and minecraft:tick instead of current namespace
#load
void main() {
    int x = 4;
    var b = true; // Type inference
    string str = "Yay";
    object obj = { 
        int prop1 = 4;
        string prop2 = str;
    };
    int[] arr = [1, 2, 3, 4, 5];

    // Command interpolation
    @/say $(str)

    var t = Test(15); // Stack alloc
    !print(t.Method()); // ! means exit function if error
}
```
