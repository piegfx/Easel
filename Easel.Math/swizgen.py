if __name__ == "__main__":
    components = ['X', 'Y']

    for comp in range(len(components)):
        for comp2 in range(len(components)):
            print(f"public Vector2T<T> {components[comp]}{components[comp2]} => new Vector2T<T>({components[comp]}, {components[comp2]});")
            print()

    for comp in range(len(components)):
        for comp2 in range(len(components)):
            for comp3 in range(len(components)):
                print(f"public Vector3T<T> {components[comp]}{components[comp2]}{components[comp3]} => new Vector3T<T>({components[comp]}, {components[comp2]}, {components[comp3]});")
                print()

    for comp in range(len(components)):
        for comp2 in range(len(components)):
            for comp3 in range(len(components)):
                for comp4 in range(len(components)):
                    print(f"public Vector4T<T> {components[comp]}{components[comp2]}{components[comp3]}{components[comp4]} => new Vector4T<T>({components[comp]}, {components[comp2]}, {components[comp3]}, {components[comp4]});")
                    print()