import urllib, json
from datetime import *
from math import pow,sqrt
from pprint import pprint

# Get currency data from API
def get_json_data(url):
    response = urllib.urlopen(url)
    data = json.loads(response.read())
    return data


# Default currencies base type 'Euro', this func convert 'TL'
def convert_currencies_tl(data):
    currency = data['rates']
    euro = data['rates']['TRY']
    for name, price in currency.iteritems():
        currency[name] = euro/price
    currency['EUR'] = euro
    del currency['TRY']
    return currency


# that attracts the exchange rates of the desired date
def get_specific_data(interval):
    total_date = {}
    for day in xrange(interval):
        total_date[str(datetime.now().date() - timedelta(day))] = convert_currencies_tl(
            get_json_data('http://api.fixer.io/latest' + str(datetime.now().date() - timedelta(day)))
        )
    return total_date


# calculates the correlation coefficient of each data set
def calculate_correlation_in_array(currencies):
    x_ort = 0
    y_ort = 0
    for count, currency in enumerate(currencies):
        x_ort += count
        y_ort += currency
    x_ort /= len(currencies)
    y_ort /= len(currencies)

    pay_toplam = 0
    payda_toplam = 0

    for count, currency in enumerate(currencies):
        pay_toplam += (count-x_ort) * (currency - y_ort)
        payda_toplam += pow(count - x_ort, 2)

    return pay_toplam / payda_toplam


# holds information about each currency
class Currencies:
    def __init__(self, space):
        self.currencies_all_data = get_specific_data(space)
        self.currencies = {}
        self.curr_and_corel = {}

    def get_currencies(self):
        return self.currencies_all_data

    def testing(self):
        for keys, values in self.currencies_all_data.iteritems():
            for key, value in values.iteritems():
                self.currencies[key] = 0
#        self.currencies['USD'] = 10
        pprint(self.currencies)

    def calculate_correlation(self):
        for keys, values in self.currencies_all_data.iteritems():
            for key, value in values.iteritems():
                if key in self.currencies.keys():
                    self.currencies[key].append(value)
                else:
                    self.currencies[key] = []
                    self.currencies[key].append(value)
        # Check data
        # pprint(self.currencies)

    def calculate_currencies_correlation(self):
        currencies = []
        for key in self.currencies.iteritems():
            self.curr_and_corel[calculate_correlation_in_array(key[1])] = key
        for key,value in self.curr_and_corel.iteritems():
            #print "bu ilk key",key,"bu ilk value",value[0],value[1]
            current_name = value[0]
            currents = value[1]
            tmp_currency = Currencie(current_name,currents,key)
            currencies.append(tmp_currency)
        return currencies

    def get_currency_info(self, current_name):
        return self.curr_and_corel.get(current_name)


class Currencie:
    def __init__(self,name, all_data,corelation):
        self.name = name
        self.all_data = all_data
        self.corelation = corelation