/*
WARNING!
Columns order:
1: print_batch_id
2: created
3: createuser
*/

SELECT
  print_batch_id
  ,(SELECT inner_package_print_batches.created FROM package_print_batches AS inner_package_print_batches WHERE inner_package_print_batches.print_batch_id = distinct_package_print_batches.print_batch_id ORDER BY created ASC LIMIT 1) AS created
  ,createuser

FROM
  (SELECT DISTINCT print_batch_id, createuser FROM package_print_batches) AS distinct_package_print_batches
  
ORDER BY
  3 DESC

LIMIT 50   