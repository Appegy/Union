# Union struct for C#

This package provides `UnionAttribute`, which can be used to generate a container struct that can hold one of several
specified struct types. The generated struct can also implement specified in `ExposeAttribute` interfaces.
Key Features:

- UnionAttribute: Specifies that the container struct can hold only one of the specified struct types at a time.
- ExposeAttribute: Specifies that the container struct should implement the specified interfaces, delegating the
  implementation to the currently held struct.

## Usage

### Defining the Container Struct

Define the container struct using the `UnionAttribute` and `ExposeAttribute` attributes:

```csharp
using Appegy.Union.Cells.Variants;

namespace Appegy.Union.Cells
{
    [Union(typeof(VoidCell), typeof(EmptyCell), typeof(RegularCell))]
    [Expose(typeof(IPuzzleCell), typeof(IMatchableCell), typeof(IMovableCell))]
    public partial struct PuzzleCell
    {
    }
}
```

### Defining the Structs and Interface

Define the structs and the interface that the container struct will implement:

```csharp
namespace Appegy.Union.Cells
{
    public interface IPuzzleCell
    {
        public short Id { get; }
    }

    public interface IMovableCell
    {
        public bool Movable { get; }
    }

    public interface IMatchableCell
    {
        public bool Matchable { get; }
    }
}

namespace Appegy.Union.Cells.Variants
{
    public struct VoidCell : IPuzzleCell, IMovableCell, IMatchableCell
    {
        public short Id => -2;
        public bool Movable => false;
        public bool Matchable => false;
    }

    public struct EmptyCell : IPuzzleCell, IMovableCell, IMatchableCell
    {
        public short Id => -1;
        public bool Movable => false;
        public bool Matchable => false;
    }

    public struct RegularCell : IPuzzleCell, IMovableCell, IMatchableCell
    {
        public short Id { get; set; }
        public bool Movable => true;
        public bool Matchable => true;

        public RegularCell(short id)
        {
            Id = id;
        }
    }
}
```

That is it. Generator will generate all necessary code.

### Using the Generated PuzzleCell Struct

#### Creating an Array of PuzzleCell and Adding Different Cell Types

```csharp
// Create an array of PuzzleCell
PuzzleCell[] puzzleCells = new PuzzleCell[3];

// Add different cell types to the array
puzzleCells[0] = new VoidCell();
puzzleCells[1] = new EmptyCell();
puzzleCells[2] = new RegularCell(42);

// Print information about the cells
foreach (var cell in puzzleCells)
{
    Debug.Log($"Type: {cell.Type}, Id: {cell.Id}, Movable: {cell.Movable}, Matchable: {cell.Matchable}");
}
```

#### Using switch to Check the Cell Type

```csharp
foreach (var cell in puzzleCells)
{
    switch (cell.Type)
    {
        case PuzzleCell.Kind.VoidCell:
            Debug.Log("This is a VoidCell");
            break;
        case PuzzleCell.Kind.EmptyCell:
            Debug.Log("This is an EmptyCell");
            break;
        case PuzzleCell.Kind.RegularCell:
            Debug.Log("This is a RegularCell with Id: " + cell.Id);
            break;
        default:
            throw new ArgumentOutOfRangeException();
    }
}
```

#### Using Implicit Conversion Operators

```csharp
// Implicit conversion from VoidCell to PuzzleCell
PuzzleCell voidCell = new VoidCell();

// Implicit conversion from PuzzleCell back to VoidCell
VoidCell extractedVoidCell = voidCell;
```

#### Working with Interfaces

```csharp
// Create a RegularCell
PuzzleCell regularCell = new RegularCell(42);

// Use the IPuzzleCell interface
IPuzzleCell puzzleCellInterface = regularCell;
Debug.Log($"IPuzzleCell Id: {puzzleCellInterface.Id}");

// Use the IMovableCell interface
IMovableCell movableCellInterface = regularCell;
Debug.Log($"IMovableCell Movable: {movableCellInterface.Movable}");

// Use the IMatchableCell interface
IMatchableCell matchableCellInterface = regularCell;
Debug.Log($"IMatchableCell Matchable: {matchableCellInterface.Matchable}");

Debug.Log($"Extracted VoidCell Id: {extractedVoidCell.Id}");
```

### Generated Code for UnionAttribute

```csharp
// <auto-generated/>
using System;
using System.Runtime.InteropServices;
using Appegy.Union.Cells.Variants;

namespace Appegy.Union.Cells
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    partial struct PuzzleCell
        : IEquatable<PuzzleCell>
        , IEquatable<VoidCell>
        , IEquatable<EmptyCell>
        , IEquatable<RegularCell>
    {
        [Serializable]
        public enum Kind : byte
        {
            VoidCell,
            EmptyCell,
            RegularCell,
        }

        [FieldOffset(0)]
        private Kind _type;
        [FieldOffset(1)]
        private VoidCell _voidCell;
        [FieldOffset(1)]
        private EmptyCell _emptyCell;
        [FieldOffset(1)]
        private RegularCell _regularCell;

        public Kind Type => _type;

        public VoidCell VoidCell
        {
            get => Type != Kind.VoidCell ? throw new Exception($"Can't get VoidCell because current type is '{Type}'.") : _voidCell;
            set
            {
                _type = Kind.VoidCell;
                _voidCell = value;
            }
        }

        public EmptyCell EmptyCell
        {
            get => Type != Kind.EmptyCell ? throw new Exception($"Can't get EmptyCell because current type is '{Type}'.") : _emptyCell;
            set
            {
                _type = Kind.EmptyCell;
                _emptyCell = value;
            }
        }

        public RegularCell RegularCell
        {
            get => Type != Kind.RegularCell ? throw new Exception($"Can't get RegularCell because current type is '{Type}'.") : _regularCell;
            set
            {
                _type = Kind.RegularCell;
                _regularCell = value;
            }
        }

        public PuzzleCell(VoidCell value)
        {
            _type = Kind.VoidCell;
            _emptyCell = default;
            _regularCell = default;
            _voidCell = value;
        }

        public PuzzleCell(EmptyCell value)
        {
            _type = Kind.EmptyCell;
            _voidCell = default;
            _regularCell = default;
            _emptyCell = value;
        }

        public PuzzleCell(RegularCell value)
        {
            _type = Kind.RegularCell;
            _voidCell = default;
            _emptyCell = default;
            _regularCell = value;
        }

        public override string ToString() => _type switch
        {
            Kind.VoidCell => _voidCell.ToString(),
            Kind.EmptyCell => _emptyCell.ToString(),
            Kind.RegularCell => _regularCell.ToString(),
            _ => throw new InvalidOperationException($"Unknown type of union: {_type}")
        };

        public override int GetHashCode() => _type switch
        {
            Kind.VoidCell => _voidCell.GetHashCode(),
            Kind.EmptyCell => _emptyCell.GetHashCode(),
            Kind.RegularCell => _regularCell.GetHashCode(),
            _ => throw new InvalidOperationException($"Unknown type of union: {_type}")
        };

        public override bool Equals(object boxed) => boxed switch
        {
            PuzzleCell other => Equals(other),
            VoidCell other => Equals(other),
            EmptyCell other => Equals(other),
            RegularCell other => Equals(other),
            _ => throw new InvalidOperationException($"Unknown type of union: {_type}")
        };

        public bool Equals(PuzzleCell other) => _type == other.Type && _type switch
        {
            Kind.VoidCell => _voidCell.Equals(other.VoidCell),
            Kind.EmptyCell => _emptyCell.Equals(other.EmptyCell),
            Kind.RegularCell => _regularCell.Equals(other.RegularCell),
            _ => throw new InvalidOperationException($"Unknown type of union: {_type}")
        };

        public bool Equals(VoidCell other) => _type == Kind.VoidCell && _voidCell.Equals(other);
        public bool Equals(EmptyCell other) => _type == Kind.EmptyCell && _emptyCell.Equals(other);
        public bool Equals(RegularCell other) => _type == Kind.RegularCell && _regularCell.Equals(other);

        public static implicit operator VoidCell(PuzzleCell other) => other.VoidCell;
        public static implicit operator PuzzleCell(VoidCell other) => new PuzzleCell(other);
        public static implicit operator EmptyCell(PuzzleCell other) => other.EmptyCell;
        public static implicit operator PuzzleCell(EmptyCell other) => new PuzzleCell(other);
        public static implicit operator RegularCell(PuzzleCell other) => other.RegularCell;
        public static implicit operator PuzzleCell(RegularCell other) => new PuzzleCell(other);

        public static bool operator ==(PuzzleCell a, PuzzleCell b) => a.Equals(b);
        public static bool operator !=(PuzzleCell a, PuzzleCell b) => !a.Equals(b);
        public static bool operator ==(PuzzleCell a, VoidCell b) => a.Equals(b);
        public static bool operator !=(PuzzleCell a, VoidCell b) => !a.Equals(b);
        public static bool operator ==(PuzzleCell a, EmptyCell b) => a.Equals(b);
        public static bool operator !=(PuzzleCell a, EmptyCell b) => !a.Equals(b);
        public static bool operator ==(PuzzleCell a, RegularCell b) => a.Equals(b);
        public static bool operator !=(PuzzleCell a, RegularCell b) => !a.Equals(b);
    }
}
```

### Generated Code for ExposeAttribute

```csharp
// <auto-generated/>
using System;
using System.Runtime.InteropServices;
using Appegy.Union.Cells.Variants;
using Appegy.Union.Cells;

namespace Appegy.Union.Cells
{
    partial struct PuzzleCell :
        IPuzzleCell,
        IMatchableCell,
        IMovableCell
    {
        #region Implement IPuzzleCell

        public short Id
        {
            get
            {
                switch (_type)
                {
                    case Kind.VoidCell: return _voidCell.Id;
                    case Kind.EmptyCell: return _emptyCell.Id;
                    case Kind.RegularCell: return _regularCell.Id;
                    default: throw new InvalidOperationException($"Unknown type of union: {_type}");
                }
            }
        }

        #endregion Implement IPuzzleCell

        #region Implement IMatchableCell

        public bool Matchable
        {
            get
            {
                switch (_type)
                {
                    case Kind.VoidCell: return _voidCell.Matchable;
                    case Kind.EmptyCell: return _emptyCell.Matchable;
                    case Kind.RegularCell: return _regularCell.Matchable;
                    default: throw new InvalidOperationException($"Unknown type of union: {_type}");
                }
            }
        }

        #endregion Implement IMatchableCell

        #region Implement IMovableCell

        public bool Movable
        {
            get
            {
                switch (_type)
                {
                    case Kind.VoidCell: return _voidCell.Movable;
                    case Kind.EmptyCell: return _emptyCell.Movable;
                    case Kind.RegularCell: return _regularCell.Movable;
                    default: throw new InvalidOperationException($"Unknown type of union: {_type}");
                }
            }
        }

        #endregion Implement IMovableCell
    }
}
```