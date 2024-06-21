# Machine Translation

After completing the [machine translation service configuration](./translate-config), you can now access the translation interface from the sidebar.

![Machine Translation Interface](../assets/en/translate-overview.png)

The machine translation interface is similar to mainstream translation tools. You input text on the left, and the translation appears on the right.

> [!TIP]
> To reduce resource consumption, Rodel Agent does not use real-time translation. Text translation needs to be manually triggered.

## Translation History

By default, Rodel Agent does not retain translation history. If you need to keep translation records, please enable `Translation History` on the settings page.

![Translation History Setting](../assets/en/translate-history-setting.png)

Once enabled, each translation will be recorded, and the data will be stored in the `trans.db` database in the working directory.