import pprint
from wolfstreet import *


total_currencies = Currencies(5)
total_currencies.calculate_correlation()
CL_currencies = total_currencies.calculate_currencies_correlation()

# Print all class content
for currentss in CL_currencies:
    print currentss.name,currentss.corelation,currentss.all_data