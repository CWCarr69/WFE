export const enumerateDaysBetweenDates = (startDate, endDate) => {
  var now = startDate.clone(),
    dates = [];

  while (now.isSameOrBefore(endDate)) {
    dates.push(now.format("YYYY-MM-DD"));
    now.add(1, "days");
  }

  return dates;
};

export const getFirstDayOfNextMonth = (date) => {
  var now = date;
  if (now.getMonth() == 11) {
      var current = new Date(now.getFullYear() + 1, 0, 1);
  } else {
      var current = new Date(now.getFullYear(), now.getMonth() + 1, 1);
  }

  return current;
}

export const getFirstDayOfPreviousMonth = (date) => {
  var now = date;
  if (now.getMonth() == 0) {
      var current = new Date(now.getFullYear() - 1, 11, 1);
  } else {
      var current = new Date(now.getFullYear(), now.getMonth() - 1, 1);
  }

  return current;
}

export const DefaultGlobalDateFormat = "MM/DD/YY";
