import xml.etree.cElementTree as ET

root = ET.Element("kurlar")
doc = ET.SubElement(root, "kur")
doc_2 = ET.SubElement(doc, "USD")

ET.SubElement(doc_2, "price").text = "3.16"
ET.SubElement(doc_2, "price").text = "3.20"
ET.SubElement(doc_2, "price").text = "3.80"
ET.SubElement(doc_2, "price").text = "3.05"
ET.SubElement(doc_2, "price").text = "3.40"

tree = ET.ElementTree(root)
tree.write("filename.xml")
