
Vue.use(BootstrapVue, {
  BFormGroup: { labelCols: 4, labelAlign: "right", labelSize: "sm", class: "mb-1" },
  BFormInput: { size: 'sm' },
  BFormRadioGroup: { size: 'sm', plain: true },
  BFormSelect: { size: 'sm', plain: true },
  BButton: { size: 'sm' },
  BFormDatepicker: { size: 'sm', buttonOnly: true, buttonVariant: "", right: true, hideHeader: true }
});
Vue.use(VueCleave);
Vue.component('v-select', VueSelect.VueSelect);
Vue.component('Splitpanes', splitpanes.Splitpanes);
Vue.component('Pane', splitpanes.Pane);

const master = {
  ccys: ["USD", "EUR", "AUD"],
  bcs: ["TKY", "LDN", "NYK"],
}

const tradeFields =  [
  { key: 'number', label: '', sortable: false, width: 30, class: "text-right" },
  { key: 'radio', label: '', sortable: false, width: 30 },
  { key: 'checked', label: '', sortable: false, width: 30 },

  { key: 'pv', width: 120 },
  { key: 'remarks', width: 120 },

  { key: 'ccy', width: 80 },
  { key: 'index', width: 120 },

  { key: 'expiry', width: 125, class: "show-swop" },
  { key: 'effective', width: 110, class: "hide-fxop" },
  { key: 'terminate', width: 110, class: "hide-fxop" },

  { key: 'buySell', label: 'BuySell', width: 70, class: "show-swop" },
  { key: 'payRec', label: 'PayRec', width: 70, class: "show-swop" },
  { key: 'capFloor', width: 70, class: "show-cap" },
  { key: 'callPut', label: 'CallPut', width: 70, class: "show-fxop" },

  { key: 'notional', width: 110, },
  { key: 'strike', width: 110, class: "show-cap show-fxop" },

  { key: 'dcc', sortable: false, width: 70 },
  { key: 'bcs', sortable: false, width: 70 },

]

function prepareTableFields(fields, sortable) {
  fields.forEach(function(field) {
    if (!field.thStyle && field.width) {
      field.thStyle =  { width: field.width + "px" }
    }
    if (field.sortable == undefined) {
      field.sortable = sortable
    } 
  })
  return fields;
}

function createItem() {
  return { 
    result: { }, 
    trade: {
      cashFlow1: [],
      cashFlow2: [],
    },
  };
}

const vm = new Vue({
  el: '.app',
  data: {
    cleave: {
      int: { numeral: true, numeralDecimalScale: 0 },
      uint: { numeral: true, numeralDecimalScale: 0, numeralPositiveOnly: true },
      decimal: { numeral: true },
      udecimal: { numeral: true, numeralPositiveOnly: true },
    },
    fieldsTrade: prepareTableFields(tradeFields, true),
    fieldsCf: [
      { key: 'index', label: '', thStyle: { width: "30px" }, thClass: "text-right", tdClass: "text-right" },
      { key: 'type', label: 'Type', sortable: true, thStyle: { width: "70px" } },
      { key: 'ccy', thStyle: { width: "80px" } },
      { key: 'expiry', thStyle: { width: "125px" } },
      { key: 'strike', thStyle: { width: "120px" }, class: "dps-option" },
      { key: 'notional', thStyle: { width: "120px" } },
    ],
    enums: {
      buySell: ["Buy", "Sell"],
      callPut: ["Call", "Put"],
      payRec: ["Pay", "Rec"],
      capFloor: ["Cap", "Floor", "Straddle"],
      dcc: ["Act365F", "Act360"],
    },
    master: null,
    items: [],
    allcheck: false,
    checkedIndex: 0,
    json : null,
    isRunning: false,
  },
  created: function(){
     this.master = master;
     this.onAdd();
  },
  watch: {
    "form.tenor": function(val) {
      //var conv = this.master.conventions.find(conv => conv.index == val);
      //this.form.dcc = conv.swapdcc;
    },
    allcheck: function(value) {
      this.items.forEach(function(item) {
        item.isChecked = value;
      });
    },
    items: function() {
      this.checkedIndex = this.checkedIndex < this.items.length ? this.checkedIndex : 0
    },
  },
  computed: {
    current: function() {
      return this.items[this.checkedIndex] || {};
    },
    cashflows: function() {
      return (this.current.cashflow1 || []).concat(this.current.cashflow2 || []);
    },
    selectedItems: function() {
      var items = this.items.filter(function(item) {
        return item.isChecked;
      });
      return items.length == 0 ? [this.current] : items;
    },
  },
  methods: {
    onSubmit: function(event) {
      var _this = this;
      this.isRunning = true;
      setTimeout(function() {
        _this.isRunning = false;
        _this.current.result = { pv: 1234.56 }
      }, 1000);
    },
    onReset: function(event) {
       this.current.trade = { bcs: [] };
       this.current.result = {};
    },
    onAdd: function() {
        this.items.push(createItem());
        this.checkedIndex = this.items.length - 1
    },
    onClone: function() {
        var items = this.items;
        this.selectedItems.forEach(function(item) {
           items.push(JSON.parse(JSON.stringify(item)));
        });
    },
    onDelete: function() {
        if (!confirm("delete?")) {
          return;
        }
        var items = this.items;
        this.selectedItems.forEach(function(item) {
           items.splice(items.indexOf(item), 1);
        });
        if (items.length == 0) {
          this.onAdd();
        }
    },
    onClear: function() {
        if (confirm("clear?")) {
          this.items.splice(0);
          this.onAdd();
        }
    },
    onExportJson: function() {
      this.json = JSON.stringify(this.selectedItems.map(function(item) {
         return item.trade;
      }), null, 2);
    },
    onImportJson: function() {
        var items = this.items;
        JSON.parse(this.json).forEach(function(trade) {
           items.push({ trade: trade, result: {} });
        });
    },
    focusNext: function(event) {
      focusNext(event);
    },
  }
});


function onNumberKey(obj, name) {
  switch(event.key) {
    case "y":
    case "b":
      obj[name] *= 1000;
    case "m":
      obj[name] *= 1000;
    case " ":
      obj[name] *= 1000;
      event.preventDefault();
  }
}
function focusNext(event) {
  var inputs = event.target.form.querySelectorAll("input,select");
  var next = inputs[Array.prototype.indexOf.call(inputs, event.target) + 1];
  if (next) {
    next.focus();
  }
}




/*
window.onload = function() {
  document.querySelectorAll(window.location.hash.replace("?", ".")).forEach(function(item) {
    //item.style.display = "none";
  });
}
*/

    /*
    options: {
      date: {
        date: true,
        delimiter: '-',
        datePattern: ['Y', 'm', 'd'],
      },
      decimal: {
        numeral: true,
        //numeralIntegerScale: 5,
        numeralDecimalScale: 5,
      }
    },
    */

/*
var cleave = new Cleave('.uint', {
    numeral: true,
    //numeralThousandsGroupStyle: 'thousand',
    numeralPositiveOnly: true
});

var cleave = new Cleave('.input-numeric', {
    numeral: true,
    numeralThousandsGroupStyle: 'thousand',
    noImmediatePrefix: true,
    rawValueTrimPrefix: true,
        numeralIntegerScale: 9,
        numeralDecimalScale: 2
});
new Cleave('.input-date', {
    date: true,
        delimiter: '-',
        datePattern: ['Y', 'm', 'd'],
});

    round: function(val, digit) {
       var d = Math.pow(10, digit);
       return val ? Math.round(val * d) / d : val;
    },
    getFirst: function(obj) {
       return obj ? obj[Object.keys(obj)[0]] : null;
    },
*/

