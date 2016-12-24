from __future__ import division
import json
import random
import urllib
import xml.etree.cElementTree as ET
from datetime import *
from math import pow
from pprint import pprint



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

    def cal_correlation_make_class_format(self):
        currencies = []
        for key in self.currencies.iteritems():
            self.curr_and_corel[calculate_correlation_in_array(key[1])] = key
        for key, value in self.curr_and_corel.iteritems():
            # print "bu ilk key",key,"bu ilk value",value[0],value[1]
            current_name = value[0]
            currents = value[1]
            tmp_currency = Currencie(current_name, currents, key)
            currencies.append(tmp_currency)
        return currencies

    def get_currency_info(self, current_name):
        return self.curr_and_corel.get(current_name)


class Currencie:
    def __init__(self, name, all_data, corelation):
        self.name = name
        self.all_data = all_data
        self.corelation = corelation
        self.order = None


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
        currency[name] = euro / price
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
        pay_toplam += (count - x_ort) * (currency - y_ort)
        payda_toplam += pow(count - x_ort, 2)

    return pay_toplam / payda_toplam


# C# use "," operator for float percenteage part. Convert
def convert_xml_format(cl_currencies):
    root = ET.Element("kurlar")

    for currentss in cl_currencies:
        doc = ET.SubElement(root, "kur")
        doc_2 = ET.SubElement(doc, "title").text = currentss.name
        for data in currentss.all_data:
            # C# use "," operator for float percenteage part
            ET.SubElement(doc, "price").text = str("%.4f" % data).replace(".", ",")
    tree = ET.ElementTree(root)
    tree.write("currencies.xml")


def sort_and_add_indis(cl_currencies):
    cl_currencies = sorted(cl_currencies, key=lambda currencie: currencie.corelation, reverse=True)
    for count, currencie in enumerate(cl_currencies):
        currencie.order = count + 1
    return cl_currencies


class Generation:
    def __init__(self, money=0, population=None, gen_size=3, population_size=10):
        self.population_size = population_size
        self.population = []
        # If the population is defined, synchronize.
        if population is not None:
            self.population = population
        # Else create new population
        elif money != 0:
            for gens in range(0, population_size):
                tmp_chromozome = Chromozome()
                for tmp_g in range(0, gen_size):
                    tmp_chromozome.add_gen(random.randint(money / (population_size * 5), money / population_size))
                self.population.append(tmp_chromozome)
                # tmp_chromozome.print_gen()
                del tmp_chromozome
        else:
            # New generation created and should do nothing...
            pass

    def get_population(self):
        return self.population

    def create_fx_values(self, currencies):
        best_currencie_values = []
        for currencie in currencies:
            # Get all currencies last values
            best_currencie_values.append(currencie.all_data.pop())

        for chromozome in self.population:
            tmp_fi = 0
            for gen, best_value in zip(chromozome.gens, best_currencie_values):
                tmp_fi = tmp_fi + gen * best_value
            chromozome.fi = tmp_fi


class Chromozome:
    def __init__(self):
        self.gens = []
        self.fi = None
        self.fitness_value = 0.0
        self.choose_possibility = 0.0

    def add_gen(self, gen):
        self.gens.append(gen)

    def return_gen(self):
        return self.gens

    def print_gen(self):
        for gen in self.gens:
            print gen,
        print "\n"


# For roulette method
def assign_fitness_values(chromozomes):
    chromozome_length = len(chromozomes)
    for order, chromozome in enumerate(chromozomes):
        fitness_value = 1 * ((chromozome_length - (order+1)) / (chromozome_length - 1))
        chromozome.fitness_value = float(format(fitness_value, '.3f'))
        print chromozome.fitness_value


def roulette(population):
    total_fx = 0.0
    choose_random = []
    after_generation = Generation()
    pprint(after_generation.population)
    for tmp in range(0, len(population)):
        choose_random.append(float(format(random.uniform(0, 1), '.3f')))
    for tmp_chromozome in population:
        total_fx += tmp_chromozome.fitness_value
    for tmp_c in population:
        tmp_c.choose_possibility = tmp_c.fitness_value / total_fx




