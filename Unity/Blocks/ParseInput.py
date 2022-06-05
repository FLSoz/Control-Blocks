import json

# Defining main function
def main():
    with open("InputBlocks.json") as file:
        blocks = json.load(file)
        for block in blocks:
            block_unity_file = f"{block['Unity_ID']}.asset"
            block_json_file = f"{block['Unity_ID']}.json"
            
            write_nuterra = False
            nuterra_block = { "AutoImported": True }
            
            def write_parameter(param):
                if param in block:
                    nuterra_block[param] = block[param]
                    return True
                return False
            
            write_nuterra = write_parameter("Price") or write_nuterra
            write_nuterra = write_parameter("HP") or write_nuterra
            write_nuterra = write_parameter("Mass") or write_nuterra
            write_nuterra = write_parameter("ID") or write_nuterra
            write_nuterra = write_parameter("Fragility") or write_nuterra
            
            if "NuterraBlock" in block:
                write_nuterra = True
                for key, value in block["NuterraBlock"].items():
                    nuterra_block[key] = value
            
            module = block["Module"]
            
            output = {}
            module_key = module["Name"]
            output[module_key] = {}
            for key, value in module.items():
                if key != "Name":
                    output[module_key][key] = value
            
            if write_nuterra:
                output["NuterraBlock"] = nuterra_block
            
            output["Recipe"] = block["Recipe"].strip(", ").lower()
            
            with open(block_json_file, "w") as outfile:
                json.dump(output, outfile, indent=4)

# Using the special variable 
# __name__
if __name__=="__main__":
    main()