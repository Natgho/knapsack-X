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
def calculate_correlation(currencies):
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

    def get_currencies(self):
        return self.currencies_all_data

    def testing(self):
        for keys, values in self.currencies_all_data.iteritems():
            for key, value in values.iteritems():
                self.currencies[key] = 0
        self.currencies['USD'] = 10
        pprint(self.currencies)

    def calculate_correlation(self):
        for keys, values in self.currencies_all_data.iteritems():
            for key, value in values.iteritems():
                if key in self.currencies.keys():
                    self.currencies[key].append(value)
                else:
                    self.currencies[key] = []
                    self.currencies[key].append(value)
        pprint(self.currencies)



# def calculate_standart_deviation(currencies):
#     total_currency = 0
#     deviation = 0
#     for count,currency in enumerate(currencies):
#         total_currency += currency
#     print total_currency
#     aritmethic_avg = total_currency / len(currencies)
#     print aritmethic_avg
#     for currency in currencies:
#         deviation += pow(currency - aritmethic_avg, 2)
#     deviation /= len(currencies) - 1
#
#     return sqrt(deviation)