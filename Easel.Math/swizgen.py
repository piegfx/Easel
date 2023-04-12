def process(components, num_components, text, final_list):
    if num_components <= 0:
        return text;

    for comp in components:
        final_list.append(text + comp)
        process(components, num_components - 1, text + comp, final_list)

if __name__ == "__main__":
    components = ['X', 'Y', 'Z', 'W']
    max_components = 4

    final_list = []
    process(components, max_components, "", final_list)
    final_list = sorted(final_list, key=lambda x: len(x))
    for elem in final_list:
        split_elem = list(elem)
        length = len(split_elem)
        joj = ", ".join(split_elem)
        print(f"public Vector{length}T<T> {elem} => new Vector{length}T<T>({joj})")